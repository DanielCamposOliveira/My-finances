using Cloud_API.src.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Cloud_API.src.Service.Upload
{
    public interface IUploadService
    {
        public Task<IResult> UploadBase(string token, [FromForm] UploadChunkDTO request, HttpContext context);
    }
}
