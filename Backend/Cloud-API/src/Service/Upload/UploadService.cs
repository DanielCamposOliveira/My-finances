using Cloud_API.src.Model.Upload;
using Cloud_API.src.Process.Upload;
using Microsoft.AspNetCore.Mvc;

namespace Cloud_API.src.Service.Upload
{
    public class UploadService : IUploadService
    {
        private readonly UrlUploadRepository _UrlUploadRepository; 
        private readonly UploadProcess _uploadProcess;


        public UploadService(UrlUploadRepository urlCache, UploadProcess uploadProcess)
        {
            _UrlUploadRepository = urlCache;
            _uploadProcess = uploadProcess;
        }




        //public IResult GetAllCache()
        //{
        //    return Results.Ok(_urlCache.GetAll());

        //}

        //public IResult GetSingleCache(string token)
        //{
        //    return Results.Ok(_urlCache.GetSingle(token));
        //}

        public async Task<IResult> UploadBase(string token, [FromForm] UploadChunkRequestModel request, HttpContext context )
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
            var arquivoNoCache = _UrlUploadRepository.GetSingle(token);

            // Se o GetSingle retornou null, significa que o token expirou ou nunca existiu
            if (arquivoNoCache == null)
            {
                return Results.NotFound("Url inválida ou token expirado!");
            }

            //Chama a função de receber o upload do arquivo
            return await _uploadProcess.Upload(token, request, context);

        }

       
    }
}
