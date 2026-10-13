using Cloud_API.src.Process.Download;

namespace Cloud_API.src.Service.Download
{
    public class DownloadService
    {
        private readonly UrlDownloadRepository _urlDownloadRepository;
        private readonly DownloadProcess _downloadBaseServiceMd5;

        public DownloadService(UrlDownloadRepository urlDownloadRepository, DownloadProcess downloadBaseServiceMd5)
        {
            _urlDownloadRepository = urlDownloadRepository;
            _downloadBaseServiceMd5 = downloadBaseServiceMd5;
        }

        public async Task<IResult> DownloadBase(string token, HttpContext context)
        {
            //verifica se o ID não esta em branco
            if (string.IsNullOrEmpty(token))
                return Results.BadRequest("TokenBase não informado.");

            // 2. Busca no Cache usando seu método GetSingle
            var arquivoNoCache = _urlDownloadRepository.GetSingle(token);

            // Se o GetSingle retornou null, significa que o token expirou ou nunca existiu
            if (arquivoNoCache == null)
            {
                return Results.NotFound("Url inválida ou token expirado!");
            }


            return await _downloadBaseServiceMd5.DownloadBase(arquivoNoCache, context);

            



        }
    }
}
