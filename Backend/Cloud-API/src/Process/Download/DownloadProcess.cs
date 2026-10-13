using System.Security.Cryptography;

namespace Cloud_API.src.Process.Download
{
    public class DownloadProcess : IDownloadProcess
    {
        private readonly string RepositoryBase;
        private readonly UrlDownloadRepository _urlDownloadRepository;

        public DownloadProcess(IConfiguration configuration, UrlDownloadRepository urlDownloadRepository)
        {
            RepositoryBase = configuration.GetSection("LocalSettings:PathStrings:BasePathRepositoryBase").Value;
            _urlDownloadRepository = urlDownloadRepository;
        }

        public async Task<IResult> DownloadBase(UrlDownload data, HttpContext context)
        {
            try
            {
                if (string.IsNullOrEmpty(data.FileName))
                {
                    return Results.BadRequest("");
                }

                var filePath = Path.Combine(RepositoryBase, data.FileName + ".pdf");

                if (!File.Exists(filePath))
                {
                    return Results.NotFound("");
                }

                // 1. Calcula o MD5 do arquivo inteiro (opcional, caso queira manter o header X-Chunk-MD5)
                string fileMd5;
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        byte[] hashBytes = await md5.ComputeHashAsync(stream);
                        fileMd5 = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                    }
                }

                _urlDownloadRepository.Remove(data.Token);

                // 2. Adiciona o MD5 nos headers da resposta
                context.Response.Headers["X-File-MD5"] = fileMd5;

                // 3. Retorna o arquivo PDF completo de forma otimizada pelo ASP.NET Core
                return Results.File(filePath, "application/pdf", enableRangeProcessing: false);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Erro ao processar download do arquivo: {ex.Message}");
            }
        }

    }
}