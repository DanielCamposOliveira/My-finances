namespace Cloud_API.src.Process.Download
{
    public interface IDownloadProcess
    {

        //   Task<IResult> DownloadBase(UrlDownload data);

        Task<string?> DownloadBase(UrlDownload data);
    }
}
