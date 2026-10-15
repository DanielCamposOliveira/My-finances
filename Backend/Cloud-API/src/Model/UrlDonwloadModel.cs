namespace Cloud_API.src.Model
{
    public class UrlDonwloadModel
    {
        public string Token { get; set; }
        public string FileName { get; set; }

        public string Url { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
