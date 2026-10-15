namespace Cloud_API.src.DTO
{
    public class UploadChunkDTO
    {
        public int ChunkIndex { get; set; } // Propriedade para receber o índice do chunk enviado, necessário para organizar os chunks na ordem correta durante o processo de merge
        public int TotalChunks { get; set; } // Propriedade para receber o número total de chunks, necessário para determinar quando iniciar o processo de merge

        public string FileName { get; set; } // Propriedade para receber o nome do arquivo final, necessário para o processo de merge
        public string extension { get; set; }

        public string chunkMd5 { get; set; } // Propriedade para receber o hash MD5 do chunk enviado, para validação de integridade
        public IFormFile File { get; set; } // Propriedade para receber o arquivo chunk enviado, necessário para salvar o chunk individualmente             
    }
}
