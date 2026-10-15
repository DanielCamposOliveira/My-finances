using Cloud_API.src.DTO;
using Cloud_API.src.Model;
using Cloud_API.src.Process.Download;
using Cloud_API.src.Repository.DataBase;

namespace Cloud_API.src.Service.Download
{
    public class DownloadService : IDownloadService
    {

        private readonly CacheDownloadRepository _CacheDownloadRepository;
        private readonly IDownloadProcess _downloadProcess;
        private readonly IDataBaseRepository _dataBaseRepository;


        public DownloadService(CacheDownloadRepository cacheDownloadRepository, IDownloadProcess downloadProcess, IDataBaseRepository dataBaseRepository)
        {
            _CacheDownloadRepository = cacheDownloadRepository;
            _downloadProcess = downloadProcess;
            _dataBaseRepository = dataBaseRepository;
        }


        public async Task<IResult> DownloadBase(string token, HttpContext httpContext)
        {
            if (string.IsNullOrEmpty(token))
                return Results.BadRequest();

            // verifica se essa URL existe no banco de dados de cache
            UrlDonwloadModel UrlNoCache = _CacheDownloadRepository.GetSingle(token);

            if (UrlNoCache == null)
            {
                return Results.NotFound();
            }


            // verifica se esse ARQUIVO existe no banco de dados
            DocumentoDownloadDTO dados = await _dataBaseRepository.ObterRegistro(UrlNoCache.FileName);

            if(dados == null)
            {
                return Results.BadRequest();
            }


            string NameArchived = dados.NameArchived + dados.Extension;
      
            var filePath = await _downloadProcess.DownloadBase(NameArchived);

            if (filePath == null)
            {

                bool var = await _dataBaseRepository.Active(dados.NameArchived, false);

                return Results.NotFound();
            }


            // Remove a URL para não ser acessada novamente
            _CacheDownloadRepository.Remove(token);

            // Atualiza o campo "DateAccessed" ultama vez acessado
            bool Update = await _dataBaseRepository.UpdateDateAccessed(dados.NameArchived);

            string fileDownloadName = dados.NameOriginal + dados.Extension;

            // Força o navegador a exibir no formato inline (visualizar) com o nome original
            httpContext.Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileDownloadName}\"");

            return Results.File(filePath, "application/pdf", enableRangeProcessing: false);

        }
    }
}
