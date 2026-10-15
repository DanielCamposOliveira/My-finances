using Cloud_API.src.Model;

namespace Cloud_API.src.Service.Url
{
    public class UrlService : IUrlService
    {
        private readonly UrlUploadRepository _urlUploadRepository; 
        private readonly UrlDownloadRepository _urlDownloadRepository;

        public UrlService(UrlUploadRepository urlUploadRepository, UrlDownloadRepository urlDownloadRepository)
        {
            _urlUploadRepository = urlUploadRepository;
            _urlDownloadRepository = urlDownloadRepository;

        }


        // Gera uma URL para o usuario fazer o UPload do PDF sem autenticação 
        public async Task<IResult> UrlGenerateUpload(HttpContext context)
        {
            // Gerar um token para validação do arquivo no banco de dados
            // gera um nome para pasta temporaria para upload do arquivo
            //string _nomePasta = Guid.NewGuid().ToString("N");
            string _nomePasta = $"{Guid.NewGuid().ToString("N")[..5]}";


            // Gerar um token para validação da url upload
            var UploadToken = Guid.NewGuid().ToString("N")[..8];


            var UrlUpload = $"{context.Request.Scheme}://{context.Request.Host}" + $"{StreamsRoutes.BaseMedia}{StreamsRoutes.UploadBase.BaseNoToken}/{UploadToken}";

            // Definir a data de expiração para 5 minutos a partir do momento da geração
            var expiresAt = DateTime.UtcNow.AddMinutes(1);

            // Criar o objeto a ser armazenado no cache
            var data = new UrlUploadModel
            {
                NomePasta =_nomePasta,
                Url = UrlUpload,
                ExpiresAt = expiresAt
            };

            // SALVANDO NO CACHE: 
            // O método 'Add' do seu UrlService usa o AbsoluteExpiration.
            // Assim que passar 10 minutos, o cache remove o objeto automaticamente.
            _urlUploadRepository.Add(UploadToken, data);



            return Results.Json(UrlUpload, statusCode: StatusCodes.Status201Created);
        }

        // Gera uma URL para o usuario fazer o Download do PDF sem autenticação 
        public async Task<IResult> UrlGenerateDownload(string token, HttpContext context)
        {  
            // Gerar um token para validação da url upload
            var DownloadToken = Guid.NewGuid().ToString("N")[..8];

            var Url = $"{context.Request.Scheme}://{context.Request.Host}" +
                            $"{StreamsRoutes.BaseArchive}{StreamsRoutes.DownloadBase.BaseNoToken}/{DownloadToken}";

            // Definir a data de expiração para 5 minutos a partir do momento da geração
            var expiresAt = DateTime.UtcNow.AddMinutes(1);

            // Criar o objeto a ser armazenado no cache
            var data = new UrlDownload
            {
                Token = DownloadToken,
                FileName = token,
                Url = Url,
                ExpiresAt = expiresAt
            };

            // SALVANDO NO CACHE: 
            // O método 'Add' do seu UrlService usa o AbsoluteExpiration.
            // Assim que passar 10 minutos, o cache remove o objeto automaticamente.
            _urlDownloadRepository.Add(data);


            return Results.Json(
                new { URL = data.Url },
                        statusCode: StatusCodes.Status201Created
            );


        }
    }
}
 