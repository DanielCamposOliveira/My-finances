namespace Cloud_API.src.Model
{
    public class UrlUploadModel
    {
        public string NomePasta { get; set; }  // Nome da pasta C:\API\DataTransfer\RepositoryCache\123456 onde monta os pedaços do arquivo
        public string Url { get; set; } // Url para chamar a rota de upload
        public DateTime ExpiresAt { get; set; } // data de expiração da url
    }
}
