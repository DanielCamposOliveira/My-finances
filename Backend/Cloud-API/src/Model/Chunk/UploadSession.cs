using static System.Net.Mime.MediaTypeNames;

namespace Cloud_API.src.Model.Chunk
{
    public class UploadSession
    {
        // HashSet garante que cada índice de chunk seja registrado apenas uma vez, mesmo em caso de reenvio
        public HashSet<int> ChunksRecebidos { get; } = new HashSet<int>();

        // Armazena a última vez que a sessão foi tocada para podermos limpar uploads abandonados posteriormente
        public DateTime UltimaAtividade { get; set; } = DateTime.UtcNow;
    }


    //    [Pedaço Chega no Controller]
    //         │
    //         ▼
    //Valida se o arquivo chegou corretamente (tamanho > 0 e não nulo)
    //         │
    //         ├──► Se NÃO bater: Retorna Erro(Front reenvia o pedaço)
    //         │
    //         ▼ (Se bater)
    //         [Barreira de Duplicidade: Verifica se o chunk já foi registrado no CacheTracker]
    //         │
    //         ├──► Se SIM: Retorna "Ignorando reenvio" (completed: false, ChunkStatus: "Duplicate")
    //         │
    //         ▼ (Se NÃO)
    //[Valida o MD5 do Arquivo recebido com o ChunkMd5 enviado]
    //         │
    //         ├──► Se NÃO bater: Retorna Erro(Front reenvia o pedaço)
    //         │
    //         ▼ (Se bater)
    //[Salva o arquivo.part no Disco]
    //         │
    //         ▼
    //[Chama o CacheTracker: RegistrarChunk()]
    //         │
    //         ▼
    //[Chama o CacheTracker: VerificarSeConcluiu()]
    //         │
    //         ├──► Se FALSE: Retorna "Chunk recebido com sucesso" (completed: false)
    //         │
    //         ▼ (Se TRUE)
    //[Entra no _mergeLock->Junta os arquivos->Limpa Disco->Limpa CacheTracker]
    //         │
    //         ▼
    //[Retorna Sucesso Absoluto(completed: true)]


}
