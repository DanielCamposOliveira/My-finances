using Cloud_API.src.DTO;
using Cloud_API.src.Process.Upload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;

namespace Cloud_API.src.Service.Upload
{
    public class UploadService : IUploadService
    {
        private readonly CacheUploadRepository _UrlUploadRepository; 
        private readonly IUploadProcess _uploadProcess;


        public UploadService(CacheUploadRepository urlCache, IUploadProcess uploadProcess)
        {
            _UrlUploadRepository = urlCache;
            _uploadProcess = uploadProcess;
        }


        public async Task<IResult> UploadBase(string token, [FromForm] UploadChunkDTO request, HttpContext context )
        {
            //verifica se o token não esta em branco
            if (string.IsNullOrEmpty(token))
                return Results.BadRequest("token não informado.");


            // Busca no Cache usando seu método GetSingle
            var arquivoNoCache = _UrlUploadRepository.GetSingle(token);
            // Se o GetSingle retornou null, significa que o token expirou ou nunca existiu
            if (arquivoNoCache == null)
            {
                return Results.NotFound("token inexistente!");
            }

            // Chama a função centralizada de validação
            var erroValidacao = await ValidarParametrosAsync(request);
            if (erroValidacao != null)
            {
                return erroValidacao; // Retorna o BadRequest ou NotFound correspondente
            }


            //Chama a função de receber o upload do arquivo
            return await _uploadProcess.Upload(token, request, context);

        }

        private async Task<IResult?> ValidarParametrosAsync(UploadChunkDTO request)
        {
            // Valida o ChunkIndex
            if (request.ChunkIndex <= 0)
            {
                return Results.BadRequest("ChunkIndex não informado ou inválido.");
            }

            if (request.TotalChunks <= 0)
            {
                return Results.BadRequest("TotalChunks não informado ou inválido.");
            }

            if (string.IsNullOrEmpty(request.FileName))
            {
                return Results.BadRequest("FileName não informado."); // Corrigido a mensagem
            }

            if (string.IsNullOrEmpty(request.extension))
            {
                return Results.BadRequest("extension não informado.");
            }

            // Verifica se a extensão não começa com o caractere '.'
            if (!request.extension.StartsWith('.'))
            {
                return Results.BadRequest("A extensão deve conter o ponto inicial (ex: .pdf).");
            }

            if (string.IsNullOrEmpty(request.chunkMd5))
            {
                return Results.BadRequest("chunkMd5 não informado.");
            }

            if (request.File == null || request.File.Length == 0)
            {
                return Results.BadRequest("File não informado.");
            }

            // Limite por chunk individual de 5 MB
            long tamanhoMaximoBytes = 5 * 1024 * 1024;

            if (request.File.Length > tamanhoMaximoBytes)
            {
                return Results.BadRequest($"O tamanho do chunk excede o limite máximo permitido de {tamanhoMaximoBytes / 1024 / 1024} MB.");
            }

            // 2. Se você quiser estimar o tamanho total baseado no TotalChunks 
            // (Nota: Isso pressupõe que os chunks têm tamanhos similares, exceto possivelmente o último)
            long tamanhoMaximoArquivoTotalBytes = 30 * 1024 * 1024; // Exemplo: Arquivo todo não pode passar de 50 MB

            // Estimativa grosseira baseada no chunk atual * TotalChunks
            long tamanhoEstimadoTotal = request.File.Length * request.TotalChunks;
            if (tamanhoEstimadoTotal > tamanhoMaximoArquivoTotalBytes)
            {
                return Results.BadRequest($"O tamanho estimado do arquivo completo excede o limite {tamanhoMaximoArquivoTotalBytes / 1024 /1024} MB permitido.");
            }

            // Validação de segurança: Verifica o conteúdo real dos bytes do arquivo
            var erroTipoArquivo = await ValidarTipoRealArquivoAsync(request.File);
            if (erroTipoArquivo != null)
            {
                return erroTipoArquivo;
            }

            return null; // Tudo OK com os parâmetros avaliados
        }

        private async Task<IResult?> ValidarTipoRealArquivoAsync(IFormFile file)
        {
            // Lê os primeiros bytes do arquivo (geralmente os primeiros 8 bytes são suficientes)
            var headerBytes = new byte[8];
            using (var stream = file.OpenReadStream())
            {
                await stream.ReadAsync(headerBytes, 0, headerBytes.Length);
            }

            // 1. Valida PDF (Assinatura: %PDF -> 0x25, 0x50, 0x44, 0x46)
            bool isPdf = headerBytes[0] == 0x25 && headerBytes[1] == 0x50 &&
                         headerBytes[2] == 0x44 && headerBytes[3] == 0x46;

            // 2. Valida JPEG (Assinatura: FF D8 FF)
            bool isJpeg = headerBytes[0] == 0xFF && headerBytes[1] == 0xD8 && headerBytes[2] == 0xFF;

            // 3. Valida PNG (Assinatura: 89 50 4E 47 0D 0A 1A 0A)
            bool isPng = headerBytes[0] == 0x89 && headerBytes[1] == 0x50 &&
                         headerBytes[2] == 0x4E && headerBytes[3] == 0x47;

            // 4. Valida DOCX / XLSX / ZIP (Arquivos Office modernos são compactados em ZIP - Assinatura: PK -> 50 4B 03 04)
            bool isZipOffice = headerBytes[0] == 0x50 && headerBytes[1] == 0x4B &&
                               headerBytes[2] == 0x03 && headerBytes[3] == 0x04;

            // Se não corresponder a nenhum dos tipos permitidos na sua aplicação:
            if (!isPdf && !isJpeg && !isPng && !isZipOffice)
            {
                return Results.BadRequest("O tipo real do arquivo é inválido ou não é suportado.");
            }

            return null; // Arquivo passou na validação de bytes reais
        }

    }
}
