namespace Cloud_API.src.Process.Download
{
    public class DownloadProcess : IDownloadProcess
    {
        private readonly string RepositoryBase;
       
        public DownloadProcess(IConfiguration configuration, CacheDownloadRepository urlDownloadRepository)
        {
            RepositoryBase = configuration.GetSection("LocalSettings:PathStrings:BasePathRepositoryBase").Value;
        }

        public async Task<string?> DownloadBase(string NameArchived)
        {

            var filePath = Path.Combine(RepositoryBase, NameArchived);

            if (!File.Exists(filePath))
            {
                return null;
            }   

            return await Task.FromResult(filePath);
        }
    }
}