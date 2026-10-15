namespace Cloud_API.src.Process.Download
{
    public class DownloadProcess : IDownloadProcess
    {
        private readonly string RepositoryBase;
       

        public DownloadProcess(IConfiguration configuration, UrlDownloadRepository urlDownloadRepository)
        {
            RepositoryBase = configuration.GetSection("LocalSettings:PathStrings:BasePathRepositoryBase").Value;
        }

        public async Task<IResult> DownloadBasesss(UrlDownload data)
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


                        
                // 3. Retorna o arquivo PDF completo de forma otimizada pelo ASP.NET Core
                return Results.File(filePath, "application/pdf", enableRangeProcessing: false);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Erro ao processar download do arquivo: {ex.Message}");
            }
        }

        // Retorna o caminho do arquivo (string) ou null se não encontrar
        public async Task<string?> DownloadBase(UrlDownload data)
        {

            var filePath = Path.Combine(RepositoryBase, data.FileName + ".pdf");

            if (!File.Exists(filePath))
            {
                return null;
            }

            // Remove o token de uso único
           // _urlDownloadRepository.Remove(data.Token);

            return await Task.FromResult(filePath);
        }

    }
}