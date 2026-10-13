using Cloud_API.src.Model.Upload;
using Microsoft.AspNetCore.Mvc;

namespace Cloud_API.src.Service.Upload
{
    public interface IUploadService
    {
        public Task<IResult> UploadBase(string token, [FromForm] UploadChunkRequestModel request, HttpContext context);
    }
}
