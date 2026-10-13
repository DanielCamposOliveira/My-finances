namespace Cloud_API.src.Process.Download
{
    public interface IDownloadProcess
    {

        Task<IResult> DownloadBase(UrlDownload data, HttpContext context);
    }
}
