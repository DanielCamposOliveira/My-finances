using Cloud_API.src.DTO;

namespace Cloud_API.src.Process.Upload
{
    public interface IUploadProcess
    {
        public Task<IResult> Upload(string token, UploadChunkDTO request, HttpContext context);
    }
}
