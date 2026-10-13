using Cloud_API.src.Model.Chunk;
using System.Collections.Concurrent;

namespace Cloud_API.src.Repository.Chunk
{
    public class ChunkRepository
    {
        private readonly ILogger<ChunkRepository> _logger;
        public ChunkRepository(ILogger<ChunkRepository> logger)
        {
            _logger = logger;
        }
        // O dicionário thread-safe que funciona como o nosso mini banco de dados em memória
        private readonly ConcurrentDictionary<string, UploadSession> _sessoes = new();

        /// <summary>
        /// Registra um pedaço recebido com sucesso no cache.
        /// 
        /// Toda vez que um pedaço passar na validação de integridade (MD5), a API chama esse método.
        /// Ele procura pelo TokenVideoBase no dicionário. Se não existir (primeiro pedaço), ele cria a sessão.
        /// Ele adiciona o chunkIndex no HashSet.
        /// Ele atualiza a UltimaAtividade para o horário atual.
        /// </summary>
        public void RegistrarChunk(string tokenVideoBase, int chunkIndex)
        {
            // o tokenVideoBase é a chave única e o chunkIndex é o número do pedaço recebido
            // O GetOrAdd garante que se a sessão não existir (primeiro chunk), ela é criada de forma segura.
            var sessao = _sessoes.GetOrAdd(tokenVideoBase, _ => new UploadSession());

            lock (sessao.ChunksRecebidos)
            {
                sessao.ChunksRecebidos.Add(chunkIndex);
                sessao.UltimaAtividade = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Verifica na memória RAM se um pedaço específico já foi recebido e processado com sucesso.
        /// </summary>
        public bool JaPossuiChunk(string tokenVideoBase, int chunkIndex)
        {
            if (_sessoes.TryGetValue(tokenVideoBase, out var sessao))
            {
                lock (sessao.ChunksRecebidos)
                {
                    return sessao.ChunksRecebidos.Contains(chunkIndex);
                }
            }
            return false;
        }


        /// <summary>
        /// Compara a quantidade de pedaços guardados no cache com o total esperado.
        /// 
        /// Logo após registrar o chunk, a API faz a checagem matemática:
        /// Ele pega a sessão do token e olha: ChunksRecebidos.Count == totalChunks.
        /// Retorna true se todos os pedaços chegaram, liberando a API para fazer o Merge.
        /// </summary>
        public bool VerificarSeConcluiu(string tokenVideoBase, int totalChunks)
        {
            if (_sessoes.TryGetValue(tokenVideoBase, out var sessao))
            {
                lock (sessao.ChunksRecebidos)
                {
                    return sessao.ChunksRecebidos.Count == totalChunks;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove permanentemente o upload do cache para liberar memória RAM.
        /// 
        /// Assim que o processo de Merge (juntar os arquivos) terminar com sucesso, a API chama esse método para deletar permanentemente o token do cache, 
        /// liberando a memória RAM imediatamente.
        /// 
        /// </summary>
        public void LimparSessao(string tokenVideoBase)
        {
            _sessoes.TryRemove(tokenVideoBase, out _);
        }


        /// <summary>
        /// Esse codigo é chamado periodicamente pelo serviço de limpeza (UploadCleanupService) para varrer a memória RAM e identificar uploads que ficaram inativos por muito tempo.
        /// Varre a memória RAM procurando por uploads que ficaram inativos por muito tempo.
        /// Remove do cache e retorna uma lista com os tokens deletados para limpeza de disco.
        /// </summary>
        public List<string> LimparSessoesInativas(TimeSpan maximoTempoInatividade)
        {
            var tokensExpirados = new List<string>();
            var agora = DateTime.UtcNow;

            foreach (var item in _sessoes)
            {
                //Console.WriteLine("Data: " + agora + " token: " + item.Key);

                // Extrai o token e a sessão do item do dicionário
                var token = item.Key;
                var sessao = item.Value;

                // Se o tempo desde a última atividade ultrapassar o limite, marca para remoção
                if (agora - sessao.UltimaAtividade > maximoTempoInatividade)
                {
                   // Console.WriteLine("Removendo token inativo: " + token);                    

                    // Remove do dicionário concorrente com segurança
                    if (_sessoes.TryRemove(token, out _))
                    {
                        tokensExpirados.Add(token);
                        _logger.LogWarning("Token removido UploadSessionCache: " + token);
                    }
                }
            }

            return tokensExpirados;
        }
    }
}
