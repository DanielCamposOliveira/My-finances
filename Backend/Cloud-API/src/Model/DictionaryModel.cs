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

    public class URLRepositoryBase
    {
        public string TokenVideoBase { get; set; }
        public string TokenUrlUpload { get; set; }
        public DateTime ExpiresAt { get; set; }
    }


   

    public class RegistroDelete
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string? extension { get; set; }
    }



}
