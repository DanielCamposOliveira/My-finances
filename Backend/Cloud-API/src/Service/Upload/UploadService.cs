using Cloud_API.src.DTO;
using Cloud_API.src.Process.Upload;
using Microsoft.AspNetCore.Mvc;

namespace Cloud_API.src.Service.Upload
{
    public class UploadService : IUploadService
    {
        private readonly CacheUploadRepository _UrlUploadRepository; 
        private readonly IUploadProcess _uploadProcess;


        public UploadService(CacheUploadRepository urlCache, IUploadProcess uploadProcess)
        {
            _UrlUploadRepository = urlCache;
            _uploadProcess = uploadProcess;
        }


        public async Task<IResult> UploadBase(string token, [FromForm] UploadChunkDTO request, HttpContext context )
        {
            // Busca no Cache usando seu método GetSingle
            var arquivoNoCache = _UrlUploadRepository.GetSingle(token);

            // Se o GetSingle retornou null, significa que o token expirou ou nunca existiu
            if (arquivoNoCache == null)
            {
                return Results.NotFound("token inexistente!");
            }
       
            //verifica se o token não esta em branco
            if (string.IsNullOrEmpty(token))
                return Results.BadRequest("token não informado.");

            //verifica se o md5 não esta em branco
            if (string.IsNullOrEmpty(request.chunkMd5))
                return Results.BadRequest("chunkMd5 não informado.");





            //Chama a função de receber o upload do arquivo
            return await _uploadProcess.Upload(token, request, context);

        }       
    }
}
