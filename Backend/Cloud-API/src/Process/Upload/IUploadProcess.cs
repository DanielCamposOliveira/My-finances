using Cloud_API.src.Model.Upload;

namespace Cloud_API.src.Process.Upload
{
    public interface IUploadProcess
    {
        public Task<IResult> Upload(string token, UploadChunkRequestModel request, HttpContext context);
    }
}
