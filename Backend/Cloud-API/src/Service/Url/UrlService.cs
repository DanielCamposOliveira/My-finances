using System.Security.Cryptography;

namespace Cloud_API.src.Service.Url
{
    public class UrlService : IUrlService
    {
        private readonly UrlRepository _urlCache; // Trocamos o DictionaryStorage pelo seu Service de Cache

        public UrlService(UrlRepository urlCache)
        {
            _urlCache = urlCache;
            
        }

        public async Task<IResult> GenerateUniqueToken(HttpContext context)
        {
            // Gerar um token para validação do arquivo no banco de dados
            //string _TokenVideoBase = Guid.NewGuid().ToString("N").Substring(0, 15);
            string _TokenVideoBase = Guid.NewGuid().ToString("N");
                       

            // Gerar um token para validação da url upload
            var UploadToken = Guid.NewGuid().ToString("N")[..8];

            var uploadUrl = $"{context.Request.Scheme}://{context.Request.Host}" +
                            $"{StreamsRoutes.BaseMedia}{StreamsRoutes.Upload.BaseNoToken}/{UploadToken}";

            // Definir a data de expiração para 5 minutos a partir do momento da geração
            var expiresAt = DateTime.UtcNow.AddMinutes(5);

            // Criar o objeto a ser armazenado no cache
            var data = new URLRepositoryBase
            {
                TokenVideoBase = _TokenVideoBase,
                TokenUrlUpload = uploadUrl,
                ExpiresAt = expiresAt
            };

            // SALVANDO NO CACHE: 
            // O método 'Add' do seu UrlService usa o AbsoluteExpiration.
            // Assim que passar 10 minutos, o cache remove o objeto automaticamente.
            _urlCache.Add(UploadToken, data);    

            return Results.Ok(data);
        }


    }
}
 