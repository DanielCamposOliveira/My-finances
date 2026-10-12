using Cloud_API.src.Background;
using Cloud_API.src.Background.BackDataScheduler;
using Cloud_API.src.Process.Download;
using Cloud_API.src.Process.Upload;
using Cloud_API.src.Service.Url;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cloud_API
{
    public class RoutesEndpoints : IRoutesEndpoints
    {
        private readonly ILogger<RoutesEndpoints> _logger;
        private readonly IFilaDeTrabalho _fila;
        private readonly DeleteDocumentService _serviceFile;
        private readonly DictionaryStorage _dictionaryStorage;
        private readonly IUrlService _UrlRepository;

        private readonly UrlRepository _urlCache;

        private readonly IDownloadBaseService _downloadBaseService;

      
      


        public RoutesEndpoints( DeleteDocumentService serviceFile, IFilaDeTrabalho fila, DictionaryStorage uploadBaseService, IUrlService urlRepository, UrlRepository urlCache, IDownloadBaseService downloadBaseService,  ILogger<RoutesEndpoints> logger)
        {         
            _fila = fila;
            _serviceFile = serviceFile;
  
            _urlCache = urlCache;
            _logger = logger;

            _UrlRepository = urlRepository;
            _dictionaryStorage = uploadBaseService;

            _downloadBaseService = downloadBaseService;
       

        }


        #region Os métodos de get
         
        //  !!! VERIFICAR
        public IResult GetTask(string token)
        {
            return Results.Ok(_dictionaryStorage.DicionarioDeMemoria.OrderByDescending(x => x.Value.TokenBase));
        }

        #endregion


        #region Os métodos de delete

        //public async Task<IResult> DeleteTask(string token)
        //{
        //    // verifica se existe esse registro, se existir disponibiliza o arquivo
        //    if (_dictionaryStorage.DicionarioDeMemoria.TryGetValue(token, out var registro))
        //    {
        //        RegistroDelete registroDelete = new RegistroDelete
        //        {
        //            Id = registro.TokenBase,
        //            FileName = registro.FileName,
        //            MovieFormatBase = registro.MovieFormatBase,

        //            MovieFormatReady = registro.MovieFormatReady,
        //            FileNameReady = registro.FileNameReady
        //        };

        //        return await _DeleteFile.DeleteRepositoryFull(registroDelete);
        //    }

        //    return Results.NotFound("Link de download expirado ou inválido.");
        //}

        //public async Task<IResult> DeleteTaskBase(string token)
        //{

        //    // verifica se existe esse registro, se existir disponibiliza o arquivo
        //    if (_dictionaryStorage.DicionarioDeMemoria.TryGetValue(token, out var registro))
        //    {
        //        return await _DeleteFile.DeleteRepositoryBase(token, registro.FileName + registro.MovieFormatBase);
        //    }

        //    return Results.NotFound("Link de download expirado ou inválido.");
        //}

        //public async Task<IResult> DeleteTaskReady(string token)
        //{
        //    // verifica se existe esse registro, se existir disponibiliza o arquivo
        //    if (_dictionaryStorage.DicionarioDeMemoria.TryGetValue(token, out var registro))
        //    {
        //        return await _DeleteFile.DeleteRepositoryReady(token, registro.FileNameReady + registro.MovieFormatReady);
        //    }
        //    return Results.NotFound("Link de download expirado ou inválido.");
        //}

        
        //public async Task<IResult> DeleteTaskErro(string token)
        //{

        //    // verifica se existe esse registro, se existir disponibiliza o arquivo
        //    if (_dictionaryStorage.DicionarioDeMemoria.TryGetValue(token, out var registro))
        //    {
        //        // Enfileira a tarefa. O token que a função pede será fornecido pela 'MinhaClass' (BackgroundService)
        //        await _fila.EnfileirarTarefaAsync(token => _serviceFile.DeleteFileErro(registro.TokenBase, registro.FileName + registro.MovieFormatBase, token));

        //        return Results.Ok();
        //    }

        //    return Results.NotFound("Link de download expirado ou inválido.");


        //    // verifica se existe esse registro, se existir disponibiliza o arquivo
        //    //if (_dictionaryStorage.DicionarioDeMemoria.TryGetValue(token, out var registro))
        //    //{
        //    //    return await _DeleteFile.DeleteRepositoryErro(token, registro.FileName + registro.MovieFormatBase);
        //    //}
        //    //return Results.NotFound("Link de download expirado ou inválido.");
        //}

        #endregion

        // Método para gerar a URL de upload arquivo base
        public async Task<IResult> GenerateUrl(HttpContext context)
        {
            return await _UrlRepository.GenerateUniqueToken(context);
        }


        public IResult GetAllCache()
        {
            return Results.Ok(_urlCache.GetAll());

        }

        public IResult GetSingleCache(string token)
        {
            return Results.Ok(_urlCache.GetSingle(token));
        }


        #region Os métodos de move
           
     
  

        public async Task<IResult> DeleteTaskBase(string token)
        {
            // verifica se existe esse registro, se existir disponibiliza o arquivo
            if (_dictionaryStorage.DicionarioDeMemoria.TryGetValue(token, out var registro))
            {
                // Monta o pacote
                RegistroDelete registroDelete = new RegistroDelete
                {
                    Id = registro.TokenBase,
                    FileName = registro.FileName,
                    extension = registro.extension                 
                };

                // Enfileira a tarefa. O token que a função pede será fornecido pela 'MinhaClass' (BackgroundService)
                await _fila.EnfileirarTarefaAsync(token => _serviceFile.Delete(registroDelete, token));

                return Results.Ok();
            }

            return Results.NotFound("Link de download expirado ou inválido.");
        }
          



        #endregion


        #region Os métodos de upload        

        public async Task<IResult> UploadBase(string token, [FromForm] UploadChunkRequest request, HttpContext context, UploadBaseService uploadService)
        {
            //verifica se o ID não esta em branco
            if (string.IsNullOrEmpty(request.TokenBase))
                return Results.BadRequest("TokenBase não informado.");

            //verifica se o token não esta em branco
            if (string.IsNullOrEmpty(token))
                return Results.BadRequest("token não informado.");

            //verifica se o md5 não esta em branco
            if (string.IsNullOrEmpty(request.chunkMd5))
                return Results.BadRequest("chunkMd5 não informado.");

            // 2. Busca no Cache usando seu método GetSingle
            var arquivoNoCache = _urlCache.GetSingle(token);

            // Se o GetSingle retornou null, significa que o token expirou ou nunca existiu
            if (arquivoNoCache == null)
            {
                return Results.NotFound("Url inválida ou token expirado!");
            }

            //Chama a função de receber o upload do arquivo
            return await uploadService.Upload(token,request, context);

        }

        #endregion


        #region Os métodos de download

        // Rota de download do arquivo base
        public async Task<IResult> DownloadBase(string token, HttpContext context)
        {
            // verifica se existe esse registro, se existir disponibiliza o arquivo
            if (_dictionaryStorage.DicionarioDeMemoria.TryGetValue(token, out var registro))
            {

                // Troca qualquer extensão existente pela nova
                string FileName = Path.ChangeExtension(registro.FileName, registro.extension);


                return await _downloadBaseService.DownloadBase(FileName, context);
            }

            return Results.NotFound("Link de download expirado ou inválido.");
        }

        // Rota de obter o tamanho do arquivo base para o cabeçalho Content-Length
        public async Task<IResult> DownloadBaseHead(string token, HttpContext context)
        {
            if (_dictionaryStorage.DicionarioDeMemoria.TryGetValue(token, out var registro))
            {
                string FileName = Path.ChangeExtension(registro.FileName, registro.extension);

                var fileSize = _downloadBaseService.GetFileSize(FileName);

                if (fileSize == null) return Results.NotFound();

                // Adiciona o tamanho no cabeçalho padrão do HTTP
                context.Response.ContentLength = fileSize;

                // Se for uma requisição HEAD, o .NET não enviará o corpo do JSON abaixo
                return Results.Ok();
            }

            return Results.NotFound("Link de download expirado ou inválido.");
        }


        #endregion


        #region Os métodos de Manutenção

        /// Método para carregar os dados do banco de dados para o dicionário em memória
        public async Task<IResult> CarregarDoBancoAsync()
        {
            try
            {
                using (var dbs = new AppDbContext())
                {
                    // Carrega todos os registros do banco de dados
                    var dados = await dbs.StorageRegistry.ToListAsync();

                    // Limpa o dicionário em memória antes de carregar os dados do banco para evitar duplicações ou dados desatualizados
                    _dictionaryStorage.DicionarioDeMemoria.Clear();    

                    foreach (var item in dados)
                    {
                        // Carrega os dados do banco para o dicionário em memória usando o TokenBase como chave
                        _dictionaryStorage.DicionarioDeMemoria[item.TokenBase] = item;
                    }
                    Console.WriteLine("[CarregarDoBanco] Dados carregados do banco com sucesso.");
                    return Results.Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[CarregarDoBanco] Erro ao carregar dados do banco: {ex.Message}");
                return Results.Problem("Erro ao carregar dados do banco.");
            }
        }



        #endregion
    }
}
