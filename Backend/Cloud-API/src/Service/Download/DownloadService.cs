using Cloud_API.src.Process.Download;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        //public async Task<IResult> DownloadBasesss(string token)
        //{
        //    //verifica se o ID não esta em branco
        //    if (string.IsNullOrEmpty(token))
        //        return Results.BadRequest("TokenBase não informado.");

        //    // verifica se a url esta disponivel
        //    var arquivoNoCache = _urlDownloadRepository.GetSingle(token);

        //    // Se o GetSingle retornou null, significa que o token expirou ou nunca existiu
        //    if (arquivoNoCache == null)
        //    {
        //        return Results.NotFound("Url inválida ou token expirado!");
        //    }

        //    return await _downloadProcess.DownloadBase(arquivoNoCache);
        //}

        public async Task<IResult> DownloadBase(string token)
        {
            if (string.IsNullOrEmpty(token))
                return Results.BadRequest();

            var arquivoNoCache = _urlDownloadRepository.GetSingle(token);

            if (arquivoNoCache == null)
            {
                return Results.NotFound();
            }

            if (string.IsNullOrEmpty(arquivoNoCache.FileName))
            {
                return Results.BadRequest();
            }


            // Chama o processo que agora retorna apenas o caminho do arquivo
            var filePath = await _downloadProcess.DownloadBase(arquivoNoCache);

            if (filePath == null)
            {
                return Results.NotFound();
            }

            // Remove a URL para não ser acessada novamente
            _urlDownloadRepository.Remove(token);

            // O serviço/endpoint é quem decide retornar o arquivo usando o framework web
            return Results.File(filePath, "application/pdf", enableRangeProcessing: false);
        }
    }
}
