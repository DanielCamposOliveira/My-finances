using System.Security.Cryptography;

namespace Cloud_API.src.Process.Download
{
    public class DownloadBaseServiceMd5 : IDownloadBaseService
    {
        private readonly string RepositoryBase;

        public DownloadBaseServiceMd5(IConfiguration configuration)
        {
            RepositoryBase = configuration.GetSection("LocalSettings:PathStrings:BasePathRepositoryBase").Value;
        }

        public async Task<IResult> DownloadBase(string fileName, HttpContext context)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    return Results.BadRequest("Nome do arquivo não fornecido.");
                }

                var filePath = Path.Combine(RepositoryBase, fileName);

           

                if (!File.Exists(filePath))
                {
                    return Results.NotFound("Arquivo não encontrado no servidor.");
                }

                // 1. Capturar o cabeçalho Range enviado pelo cliente (ex: "bytes=0-10485760")
                var rangeHeader = context.Request.Headers["Range"].ToString();
                if (string.IsNullOrEmpty(rangeHeader) || !rangeHeader.StartsWith("bytes="))
                {
                    return Results.BadRequest("Cabeçalho Range inválido ou ausente.");
                }

                long fileLength = new FileInfo(filePath).Length;

                // Extrai os valores de início e fim da string "bytes=inicio-fim"
                var rangeValue = rangeHeader.Replace("bytes=", "").Split('-');
                if (!long.TryParse(rangeValue[0], out long start))
                {
                    return Results.BadRequest("Formato de Range inválido.");
                }

                // Se o cliente não mandar o fim, assume o final do arquivo
                long end = (rangeValue.Length > 1 && long.TryParse(rangeValue[1], out long parsedEnd))
                    ? parsedEnd
                    : fileLength - 1;

                // Garante que os limites não ultrapassam o tamanho do arquivo
                if (end >= fileLength) end = fileLength - 1;
                if (start > end) return Results.BadRequest("Range solicitado fora dos limites.");

                long chunkSize = (end - start) + 1;
                byte[] buffer = new byte[chunkSize];

                // 2. Ler de forma precisa apenas o pedaço (chunk) solicitado do arquivo
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
                {
                    fileStream.Seek(start, SeekOrigin.Begin);
                    int totalBytesRead = 0;
                    while (totalBytesRead < chunkSize)
                    {
                        int bytesRead = await fileStream.ReadAsync(buffer, totalBytesRead, (int)chunkSize - totalBytesRead);
                        if (bytesRead == 0) break;
                        totalBytesRead += bytesRead;
                    }
                }

                // 3. Calcular o hash MD5 apenas em cima desse array de bytes (do pedaço)
                string chunkMd5;
                using (var md5 = MD5.Create())
                {
                    byte[] hashBytes = md5.ComputeHash(buffer);
                    chunkMd5 = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }

                // 4. Configurar os cabeçalhos HTTP obrigatórios para uma resposta parcial (206)
                context.Response.StatusCode = StatusCodes.Status206PartialContent;
                context.Response.ContentType = "application/octet-stream";

                // Informa ao cliente o range que está sendo entregue e o tamanho total (padrão HTTP)
                context.Response.Headers["Content-Range"] = $"bytes {start}-{end}/{fileLength}";

                // Injeta o MD5 customizado do pedaço que o cliente vai validar na memória RAM
                context.Response.Headers["X-Chunk-MD5"] = chunkMd5;

                // Descarrega o array de bytes diretamente na resposta HTTP
                await context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
                return Results.Empty;
            }
            catch (Exception ex)
            {
                return Results.Problem($"Erro ao processar download do chunk: {ex.Message}");
            }
        }

  

        // Mantemos o método original intocado para responder às requisições HEAD (MapMethods)
        public long? GetFileSize(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            var filePath = Path.Combine(RepositoryBase, fileName);

            if (File.Exists(filePath))
            {
                return new FileInfo(filePath).Length;
            }

            return null;
        }

    }
}
