namespace Cloud_API.src.Process.Download
{
    public interface IDownloadBaseService
    {
        long? GetFileSize(string fileName);

        Task<IResult> DownloadBase(string fileName, HttpContext context);
    }
}
