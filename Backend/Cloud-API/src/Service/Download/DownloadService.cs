using Cloud_API.src.Process.Download;

namespace Cloud_API.src.Service.Download
{
    public class DownloadService : IDownloadService
    {
        private readonly UrlDownloadRepository _urlDownloadRepository;
        private readonly IDownloadProcess _downloadProcess;

        public DownloadService(UrlDownloadRepository urlDownloadRepository, IDownloadProcess downloadProcess)
        {
            _urlDownloadRepository = urlDownloadRepository;
            _downloadProcess = downloadProcess;
        }

        public async Task<IResult> DownloadBase(string token, HttpContext context)
        {
            //verifica se o ID não esta em branco
            if (string.IsNullOrEmpty(token))
                return Results.BadRequest("TokenBase não informado.");

            // verifica se a url esta disponivel
            var arquivoNoCache = _urlDownloadRepository.GetSingle(token);

            // Se o GetSingle retornou null, significa que o token expirou ou nunca existiu
            if (arquivoNoCache == null)
            {
                return Results.NotFound("Url inválida ou token expirado!");
            }

            return await _downloadProcess.DownloadBase(arquivoNoCache, context);
        }
    }
}
