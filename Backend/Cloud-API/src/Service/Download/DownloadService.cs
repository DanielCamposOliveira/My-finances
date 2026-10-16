using Cloud_API.src.Background.BackgroundProcess;
using Cloud_API.src.DTO;
using Cloud_API.src.Model;
using Cloud_API.src.Process.Download;
using Cloud_API.src.Repository.DataBase;
using Cloud_API.src.Repository.Dictionary;

namespace Cloud_API.src.Service.Download
{
    public class DownloadService : IDownloadService
    {

        private readonly CacheDownloadRepository _CacheDownloadRepository;
        private readonly DictionaryRepository BancoDadosMemoria;
        
        private readonly IFilaDeTrabalho _fila;

        private readonly IDownloadProcess _downloadProcess;
        private readonly IDataBaseRepository BancoDadosLocal;


        public DownloadService(CacheDownloadRepository cacheDownloadRepository, IDownloadProcess downloadProcess, IDataBaseRepository dataBaseRepository, DictionaryRepository dictionaryRepository, IFilaDeTrabalho fila)
        {
            _CacheDownloadRepository = cacheDownloadRepository;
            _downloadProcess = downloadProcess;
            BancoDadosLocal = dataBaseRepository;

            BancoDadosMemoria = dictionaryRepository;
            _fila = fila;
        }


        public async Task<IResult> DownloadBase(string token, HttpContext httpContext)
        {
            if (string.IsNullOrEmpty(token))
                return Results.BadRequest();

            // ==========================================
            // 1. VERIFICA SE EXISTE A URL DO BD CACHE
            // ==========================================
            UrlDonwloadModel UrlNoCache = _CacheDownloadRepository.GetSingle(token);

            if (UrlNoCache == null)
            {
                return Results.NotFound();
            }


            // ==========================================
            // 2. VERIFICA SE O ARQUIVO EXISTE NO BANCO DE DADOS
            // ==========================================
            DocumentoDownloadDTO dados = await BancoDadosMemoria.ObterRegistro(UrlNoCache.FileName);

            if (dados == null)
            {
                return Results.BadRequest();
            }


            // ==========================================
            // 3. VERIFICA SE O ARQUIVO EXISTE NA PASTA
            // ==========================================
            string NameArchived = dados.NameArchived + dados.Extension;
      
            var filePath = await _downloadProcess.DownloadBase(NameArchived);

            if (filePath == null)
            {

                //REGISTRA NO BANCO DE DADOS (VIA FILA) - Update do status
                await _fila.EnfileirarTarefaAsync(token => BancoDadosLocal.Active(dados.NameArchived, false));

                //REGISTRA NO BANCO DE MEMORIA - Update do status
                await BancoDadosMemoria.Active(dados.NameArchived, false);

                return Results.NotFound();
            }

            // ==========================================
            // 4. REMOVE A URL DO BD CACHE
            // ==========================================
            _CacheDownloadRepository.Remove(token);



            // ==========================================
            // 5. REGISTRA NO BANCO DE DADOS (VIA FILA)
            // ==========================================

            //REGISTRA NO BANCO DE DADOS (VIA FILA) - UpdateDateAccessed
            await _fila.EnfileirarTarefaAsync(token => BancoDadosLocal.UpdateDateAccessed(dados.NameArchived));

            //REGISTRA NO BANCO DE MEMORIA - UpdateDateAccessed
            await BancoDadosMemoria.UpdateDateAccessed(dados.NameArchived);



            // ==========================================
            // 6. MONTA OS DADOS E REPASSA PARA O USUARIO
            // ==========================================
            string fileDownloadName = dados.NameOriginal + dados.Extension;

            // Força o navegador a exibir no formato inline (visualizar) com o nome original
            httpContext.Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileDownloadName}\"");

            return Results.File(filePath, "application/pdf", enableRangeProcessing: false);

        }
    }
}
