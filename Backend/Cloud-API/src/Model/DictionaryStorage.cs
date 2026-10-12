using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace Cloud_API
{
    public class DictionaryStorage
    {
        public  readonly ConcurrentDictionary<string, Registro> DicionarioDeMemoria = new(); // dicionário para armazenar os dados da Tarefa, onde a chave é o TokenVideoBase        
    }


    public class UploadChunkRequest
    {
        public int ChunkIndex { get; set; } // Propriedade para receber o índice do chunk enviado, necessário para organizar os chunks na ordem correta durante o processo de merge
        public int TotalChunks { get; set; } // Propriedade para receber o número total de chunks, necessário para determinar quando iniciar o processo de merge
                                             
        //public string FileName { get; set; } // Propriedade para receber o nome do arquivo final, necessário para o processo de merge
        public string extension { get; set; }

        public string TokenBase { get; set; } // Propriedade para receber o token de upload, que é a chave para acessar os dados relacionados no dicionário de memória
        public string chunkMd5 { get; set; } // Propriedade para receber o hash MD5 do chunk enviado, para validação de integridade
        public IFormFile File { get; set; } // Propriedade para receber o arquivo chunk enviado, necessário para salvar o chunk individualmente             
    }    
}
