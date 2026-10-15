namespace Cloud_API.src.Process.Download
{
    public interface IDownloadProcess
    {
        Task<string?> DownloadBase(string NameArchived);
    }
}
