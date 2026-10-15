namespace Cloud_API.src.Model
{
    public class Documento
    {
        public int Id { get; set; }
        public string NameOriginal { get; set; }
        public string NameArchived { get; set; }
        public string Extension { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime DateAccessed { get; set; } // = DateTime.UtcNow;

        public bool Ativo { get; set; } = true; // se o arquivo for apagado por algum motivo, não deletamos o registro por enquanto
    }
}
