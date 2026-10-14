namespace Cloud_API.src.Service.Download
{
    public interface IDownloadService
    {
        public Task<IResult> DownloadBase(string token, HttpContext context);
    }
}
