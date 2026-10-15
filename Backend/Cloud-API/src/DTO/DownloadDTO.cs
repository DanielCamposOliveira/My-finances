namespace Cloud_API.src.DTO
{
    public class DownloadDTO
    {
        public string Token { get; set; }
        public string FileName { get; set; }

        public string Url { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
