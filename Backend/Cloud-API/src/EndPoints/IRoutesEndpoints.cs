using Cloud_API.src.Process.Upload;
using Microsoft.AspNetCore.Mvc;

namespace Cloud_API
{
    public interface IRoutesEndpoints
    {













        // Endpoints relacionados ao cache
        IResult GetAllCache();
        IResult GetSingleCache(string token);



        IResult GetTask(string token);
             
  

        Task<IResult> UploadBase(string token, [FromForm] UploadChunkRequest request, HttpContext context, UploadBaseService uploadService);        

        Task<IResult> DownloadBaseHead(string token, HttpContext context);

        Task<IResult> DownloadBase(string token, HttpContext context);

   
        Task<IResult> CarregarDoBancoAsync();

        Task<IResult> GenerateUrl(HttpContext context);



    }
}
