using Cloud_API.src.Model.Chunk;
using Cloud_API.src.Utils;
using System.Security.Cryptography;

namespace Cloud_API.src.Process.Upload
{
    public class UploadBaseService
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

        private readonly ILogger<UploadBaseService> _logger;

        private readonly string RepositoryBase;
        private readonly string RepositoryCache;
        private readonly UrlRepository _urlCache;

        private readonly GeneratorName _GeneratorName;

        // Injeção do nosso novo banco de dados em cache dedicado
        private readonly UploadChunkTracker _chunkTracker;

        // Lock para garantir que apenas um processo de merge ocorra por vez
        private static readonly SemaphoreSlim _mergeLock = new SemaphoreSlim(1, 1);

        private readonly DictionaryStorage _dictionaryStorage;


        public UploadBaseService(IConfiguration configuration, UrlRepository urlCache, UploadChunkTracker chunkTracker, DictionaryStorage dictionaryStorage, GeneratorName generatorName, ILogger<UploadBaseService> logger)
        {
            RepositoryBase = configuration.GetSection("LocalSettings:PathStrings:BasePathRepositoryBase").Value;
            RepositoryCache = configuration.GetSection("LocalSettings:PathStrings:BasePathRepositoryCache").Value;
            _urlCache = urlCache;
            _chunkTracker = chunkTracker;
            _logger = logger;
            _dictionaryStorage = dictionaryStorage;
            _GeneratorName = generatorName;
        }

        public async Task<IResult> Upload(string token, UploadChunkRequest request, HttpContext context)
        {
            try
            {
                // Verifica se o arquivo chegou corretamente
                if(request.File.Length == 0 || request.File == null)
                {
                    return Results.BadRequest(new { completed = false, ChunkPart = request.ChunkIndex, ChunkStatus = "Erro", message = "Arquivo não enviado" });
                }

                // ====================================================================
                // BARREIRA DE DUPLICIDADE: Evita reprocessar o mesmo pedaço à toa
                // ====================================================================
                if (_chunkTracker.JaPossuiChunk(request.TokenBase, request.ChunkIndex))
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
                string tempFolderName = Path.Combine(RepositoryCache, request.TokenBase);
                if (!Directory.Exists(tempFolderName)) Directory.CreateDirectory(tempFolderName);

                // ==========================================
                // 2. SALVA O CHUNK NO DISCO
                // ==========================================e
                string chunkFilePath = Path.Combine(tempFolderName, $"{request.ChunkIndex}.part");
                using (var stream = new FileStream(chunkFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    // salva o chunk diretamente no disco sem carregar tudo na memória RAM
                    await request.File.CopyToAsync(stream);
                }

                // ==========================================
                // 3. REGISTRA E VALIDA NO TRACKER (MEMÓRIA RAM)
                // ==========================================
                _chunkTracker.RegistrarChunk(request.TokenBase, request.ChunkIndex);

                // Verifica na memória RAM (de forma ultra rápida) se TODOS os pacotes chegaram
                if (_chunkTracker.VerificarSeConcluiu(request.TokenBase, request.TotalChunks))
                {
                    // Tenta entrar no processo de Merge (apenas a thread que completou o quebra-cabeça avança)
                    if (await _mergeLock.WaitAsync(0))
                    {
                        try
                        {                            
                            string NomeGerado = _GeneratorName.CreateName();
                            // string finalFilePath = Path.Combine(RepositoryBase, request.FileName);
                            string finalFilePath = Path.Combine(RepositoryBase, NomeGerado, request.extension);

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

                            //Busca no Cache usando seu método GetSingle
                            var arquivoNoCache = _urlCache.GetSingle(token);

                            // Se o GetSingle retornou null, significa que o token expirou ou nunca existiu
                            if (arquivoNoCache == null)
                            {
                                return Results.NotFound("Url inválida ou token expirado!");
                            }


                            _dictionaryStorage.DicionarioDeMemoria[request.TokenBase] = new Registro
                            {
                                TokenBase = arquivoNoCache.TokenVideoBase,
                                FileName = NomeGerado, // salva o nome do arquivo sem extensão
                                extension = request.extension, // adiciona a extensão do arquivo original
                                DataUpload = DateTime.UtcNow,
                                IsDirty = true
                            };


                            // ==========================================
                            // 5. LIMPEZA DOS BANCOS DE CACHE
                            // ==========================================
                            _urlCache.Remove(token); // Consome o token de segurança da URL (impede que seja usado para outro upload ou download)
                            _chunkTracker.LimparSessao(request.TokenBase); // Limpa a memória RAM do Tracker

                            var downloadUrl = $"{context.Request.Scheme}://{context.Request.Host}";

                            // ==========================================
                            // 6. RETORNA O RESULTADO FINAL PARA O FRONTEND
                            // ==========================================

                            // Retorna um informação útil para o Frontend, como a URL de download do arquivo finalizado   
                            return Results.Ok(new { completed = true, NameFile = NomeGerado });


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
                _logger.LogError(ex, "Erro ao processar o chunk {ChunkIndex} do token {TokenVideoBase}", request.ChunkIndex, request.TokenBase);
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
