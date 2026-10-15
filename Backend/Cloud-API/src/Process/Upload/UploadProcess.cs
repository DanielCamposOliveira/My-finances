using Cloud_API.src.DTO;
using Cloud_API.src.Model;
using Cloud_API.src.Repository.Chunk;
using Cloud_API.src.Repository.DataBase;
using Cloud_API.src.Repository.Dictionary;
using Cloud_API.src.Utils;
using System.Security.Cryptography;

namespace Cloud_API.src.Process.Upload
{
    public class UploadProcess : IUploadProcess
    {

        /// <summary>
        /// Fluxo Completo do Upload de Arquivo em Pedaços (Chunked Upload)
        /// 
        /// 1. Camada de Rede (Duplicidade): Se o Front mandar o mesmo pedaço de novo por engano, a API barra no JaPossuiChunk (Gasto de recurso: Quase zero).
        /// 
        /// 2. Camada de Integridade (Corrupção): Se o pedaço for inédito, a API calcula o MD5. Se o arquivo sofreu mutação ou corte no meio do caminho, 
        /// a API barra no hash (Gasto de recurso: Apenas processamento de hash em memória).
        /// 
        /// 3. Camada de Armazenamento (I/O de Disco): O disco rígido só é tocado se o pedaço for inédito E estiver 100% perfeito.
        /// </summary>

        private readonly ILogger<UploadProcess> _logger;

        private readonly string RepositoryBase;
        private readonly string RepositoryCache;

        private readonly CacheUploadRepository _urlRepository;
        private readonly ChunkRepository _chunkRepository;
        private readonly GeneratorName _GeneratorName;
        private readonly DictionaryRepository _dictionaryRepository;

        // Lock para garantir que apenas um processo de merge ocorra por vez
        private static readonly SemaphoreSlim _mergeLock = new SemaphoreSlim(1, 1);

        private readonly IDataBaseRepository _dataBaseRepository;

        public UploadProcess(IConfiguration configuration, CacheUploadRepository urlCache, ChunkRepository chunkTracker, DictionaryRepository dictionaryStorage, GeneratorName generatorName, ILogger<UploadProcess> logger, IDataBaseRepository dataBaseRepository)
        {
            RepositoryBase = configuration.GetSection("LocalSettings:PathStrings:BasePathRepositoryBase").Value;
            RepositoryCache = configuration.GetSection("LocalSettings:PathStrings:BasePathRepositoryCache").Value;
            _urlRepository = urlCache;
            _chunkRepository = chunkTracker;
            _logger = logger;
            _dictionaryRepository = dictionaryStorage;
            _GeneratorName = generatorName;
            _dataBaseRepository = dataBaseRepository;
        }

        // token é passado aqui apenas para saber se a URL ainda esta valida e no fim do upload é removido do cache essa URL
        public async Task<IResult> Upload(string token, UploadChunkDTO request, HttpContext context)
        {
            try
            {
                //Busca no Cache usando seu método GetSingle
                var arquivoNoCache = _urlRepository.GetSingle(token);

                // Se o GetSingle retornou null, significa que o token expirou ou nunca existiu
                if (arquivoNoCache == null)
                {
                    return Results.NotFound("Url inválida ou token expirado!");
                }


                // Verifica se o arquivo chegou corretamente
                if (request.File.Length == 0 || request.File == null)
                {
                    return Results.BadRequest(new { completed = false, ChunkPart = request.ChunkIndex, ChunkStatus = "Erro", message = "Arquivo não enviado" });
                }

                // ====================================================================
                // BARREIRA DE DUPLICIDADE: Evita reprocessar o mesmo pedaço à toa
                // ====================================================================
                // if (_chunkRepository.JaPossuiChunk(request.TokenBase, request.ChunkIndex))
                if (_chunkRepository.JaPossuiChunk(arquivoNoCache.NomePasta, request.ChunkIndex))
                {    
                    return Results.Ok(new { completed = false, ChunkPart = request.ChunkIndex, ChunkStatus = "Duplicate", message = "Ignorando reenvio" });
                }


                // ==========================================
                // 1. VALIDAÇÃO DE INTEGRIDADE (MD5)
                // ==========================================
                string backendChunkMd5 = await CalcularMd5StreamAsync(request.File);

                // Compara o MD5 gerado com o que o Frontend enviou (ignora maiúsculas/minúsculas)
                if (!string.Equals(backendChunkMd5, request.chunkMd5, StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest(new { completed = false, ChunkPart = request.ChunkIndex, ChunkStatus = "Erro", message = "MD5 incorreto" });
                }


                // Cria o diretório temporário para os pedaços usando o TokenVideoBase como nome da pasta
                //string tempFolderName = Path.Combine(RepositoryCache, request.TokenBase);

                string tempFolderName = Path.Combine(RepositoryCache, arquivoNoCache.NomePasta);

                if (!Directory.Exists(tempFolderName)) Directory.CreateDirectory(tempFolderName);

                // ==========================================
                // 2. SALVA O CHUNK NO DISCO
                // ==========================================
                string chunkFilePath = Path.Combine(tempFolderName, $"{request.ChunkIndex}.part");
                using (var stream = new FileStream(chunkFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    // salva o chunk diretamente no disco sem carregar tudo na memória RAM
                    await request.File.CopyToAsync(stream);
                }

                // ==========================================
                // 3. REGISTRA E VALIDA NO TRACKER (MEMÓRIA RAM)
                // ==========================================
                //_chunkRepository.RegistrarChunk(request.TokenBase, request.ChunkIndex);
                _chunkRepository.RegistrarChunk(arquivoNoCache.NomePasta, request.ChunkIndex);

                // Verifica na memória se TODOS os pacotes chegaram
                //if (_chunkRepository.VerificarSeConcluiu(request.TokenBase, request.TotalChunks))
                if (_chunkRepository.VerificarSeConcluiu(arquivoNoCache.NomePasta, request.TotalChunks))
                {
                    // Tenta entrar no processo de Merge (apenas a thread que completou o quebra-cabeça avança)
                    if (await _mergeLock.WaitAsync(0))
                    {
                        try
                        {                            
                            string NomeGerado = _GeneratorName.CreateName();
                            // string finalFilePath = Path.Combine(RepositoryBase, request.FileName);
                            string finalFilePath = Path.Combine(RepositoryBase, NomeGerado + request.extension);

                            // Se o arquivo final já existir por algum motivo, deleta para evitar conflito
                            if (File.Exists(finalFilePath)) File.Delete(finalFilePath);

                            // Ordena os arquivos fisicamente com base nos índices numéricos dos nomes (.part)
                            var chunkFiles = Directory.GetFiles(tempFolderName, "*.part")
                                .OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f)));

                            // Realiza a junção (Merge) de todos os pedaços
                            using (var finalStream = new FileStream(finalFilePath, FileMode.Create))
                            {
                                foreach (var chunkFile in chunkFiles)
                                {
                                    using (var chunkStream = new FileStream(chunkFile, FileMode.Open))
                                    {
                                        await chunkStream.CopyToAsync(finalStream);
                                    }
                                }
                            }

                            // Limpeza física dos arquivos temporários
                            Directory.Delete(tempFolderName, true);


                            // ==========================================
                            // 4. REGISTRA O ARQUIVO NO BANCO DE DADOS DictionaryStorage APENAS QUANDO O MERGE FOR CONCLUÍDO COM SUCESSO
                            // ==========================================

                            ////Busca no Cache usando seu método GetSingle
                            //var arquivoNoCache = _urlRepository.GetSingle(token);

                            //// Se o GetSingle retornou null, significa que o token expirou ou nunca existiu
                            //if (arquivoNoCache == null)
                            //{
                            //    return Results.NotFound("Url inválida ou token expirado!");
                            //}


                            //_dictionaryRepository.BancoDadosMemoria[request.TokenBase] = new Documento
                            //{

                            //    //TokenBase = arquivoNoCache.TokenBase,
                            //    //FileName = NomeGerado, // salva o nome do arquivo sem extensão
                            //    //extension = request.extension, // adiciona a extensão do arquivo original
                            //    //DataUpload = DateTime.UtcNow,
                            //    //IsDirty = true
                            //};

                            // Grava no banco de dados
                            bool gravar = await _dataBaseRepository.RegistraArquivo(request, NomeGerado);
                            if(gravar == false)
                            {
                                return Results.Json(
                                    new { completed = false, NameFile = NomeGerado },
                                    statusCode: StatusCodes.Status400BadRequest
                                );
                            }
                            // ==========================================
                            // 5. LIMPEZA DOS BANCOS DE CACHE
                            // ==========================================
                            _urlRepository.Remove(token); // Consome o token de segurança da URL (impede que seja usado para outro upload ou download)
                            //_chunkRepository.LimparSessao(request.TokenBase); // Limpa a memória RAM do Tracker
                            _chunkRepository.LimparSessao(arquivoNoCache.NomePasta); // Limpa a memória RAM do Tracker


                            var downloadUrl = $"{context.Request.Scheme}://{context.Request.Host}";

                            // ==========================================
                            // 6. RETORNA O RESULTADO FINAL PARA O FRONTEND
                            // ==========================================

                            // Retorna um informação útil para o Frontend, como a URL de download do arquivo finalizado   
                           // return Results.Ok(new { completed = true, NameFile = NomeGerado });
                                                 
                            return Results.Json(
                                new { completed = true, NameFile = NomeGerado },
                                statusCode: StatusCodes.Status201Created
                            );


                        }
                        finally
                        {
                            // Garante que o lock seja liberado mesmo se ocorrer um erro durante o merge
                            _mergeLock.Release();
                        }
                    }
                }

                // Resposta padrão caso ainda faltem pedaços na fila
                return Results.Ok(new { completed = false, ChunkPart = request.ChunkIndex, ChunkStatus = "OK" , message = "" });

            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Erro ao processar o chunk {ChunkIndex} do token {TokenVideoBase}", request.ChunkIndex, request.TokenBase);
                _logger.LogError(ex, "Erro ao processar o chunk {ChunkIndex} do token {TokenVideoBase}", request.ChunkIndex);
                return Results.BadRequest(new
                {
                    completed = false,
                    ChunkPart = request.ChunkIndex,
                    ChunkStatus = "Erro",
                    message = ex.Message
                });
            }
        }


        /// <summary>
        /// Calcula o hash MD5 de um arquivo IFormFile sem carregá-lo inteiramente na memória RAM.
        /// </summary>
        private static async Task<string> CalcularMd5StreamAsync(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            using (var md5 = MD5.Create())
            {
                byte[] hashBytes = await md5.ComputeHashAsync(stream);
                // Converte os bytes do hash para uma string Hexadecimal amigável (ex: a1b2c3d4...)
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

    }
}
