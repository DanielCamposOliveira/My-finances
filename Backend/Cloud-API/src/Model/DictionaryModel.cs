namespace Cloud_API
{


    public class Registro
    {
        public int Id { get; set; }
        public string TokenBase { get; set; }
        public string FileName { get; set; }
        public string extension { get; set; }          
        public DateTime DataUpload { get; set; }
        public bool IsDirty { get; set; } = true;
    }







    public class UrlDownload
    {
        public string Token { get; set; }
        public string FileName { get; set; }

        public string Url { get; set; }
        public DateTime ExpiresAt { get; set; }
    }




}
