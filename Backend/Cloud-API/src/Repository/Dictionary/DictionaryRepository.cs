using Cloud_API.src.DTO;
using Cloud_API.src.Model;
using System.Collections.Concurrent;

namespace Cloud_API.src.Repository.Dictionary
{
    public class DictionaryRepository
    {
        // Dicionário seguro para threads (Thread-Safe)
        private readonly ConcurrentDictionary<string, Documento> _bancoDadosMemoria = new();

        // Propriedade opcional caso precise acessar o dicionário cru externamente
        public ConcurrentDictionary<string, Documento> BancoDadosMemoria => _bancoDadosMemoria;


        //Registra ou adiciona um novo documento na memoria.
        public bool Registrar(string chave, Documento documento)
        {
            // Tenta adicionar. Se a chave já existir, retorna false.
            return _bancoDadosMemoria.TryAdd(chave, documento);
        }

        //Retorna todos os documentos armazenados na memoria.
        public async Task< IEnumerable<Documento>> ConsultarTodos()
        {
            return _bancoDadosMemoria.Values;
        }


        //Consulta um documento pela chave.
        public async Task<DocumentoDownloadDTO> ObterRegistro(string nameArchived)
        {
            // Procura nos valores do dicionário onde o NameArchived seja igual ao parâmetro e esteja ativo
            var dados = _bancoDadosMemoria.Values
                .FirstOrDefault(d => d.NameArchived == nameArchived && d.Ativo);

            if (dados == null)
            {
                return null;
            }

            // Mapeia para o DTO de retorno
            var registro = new DocumentoDownloadDTO
            {
                NameArchived = dados.NameArchived,
                NameOriginal = dados.NameOriginal,
                Extension = dados.Extension,
                Ativo = dados.Ativo
            };

            return registro;
        }


        public async Task<bool> UpdateDateAccessed(string nameArchived)
        {
            try
            {
                // Encontra o documento correspondente no dicionário em memória
                var dados = _bancoDadosMemoria.Values
                    .FirstOrDefault(d => d.NameArchived == nameArchived && d.Ativo);

                if (dados == null)
                {
                    return false;
                }

                // Atualiza as propriedades diretamente na referência do objeto
                dados.DateAccessed = DateTime.Now;
                dados.QtdAcess = dados.QtdAcess + 1;

                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> Active(string nameArchived, bool valor)
        {
            try
            {
                // Encontra o documento correspondente no dicionário em memória
                var dados = _bancoDadosMemoria.Values.FirstOrDefault(d => d.NameArchived == nameArchived );

                if (dados == null)
                {
                    return false;
                }

                // Atualiza as propriedades diretamente na referência do objeto
                dados.Ativo = valor;               

                return true;
            }
            catch
            {
                return false;
            }
        }


        //Remove um documento do dicionário em memória com base no NameArchived
        public async Task<bool> Delete(string nameArchived)
        {
            try
            {
                // Localiza o par (chave e valor) onde o NameArchived corresponde e está ativo
                var itemEncontrado = _bancoDadosMemoria
                    .FirstOrDefault(d => d.Value.NameArchived == nameArchived);

                // Se não encontrar nenhum item com esse NameArchived
                if (itemEncontrado.Key == null)
                {
                    return false;
                }

                // Remove do ConcurrentDictionary usando a chave encontrada
                return _bancoDadosMemoria.TryRemove(itemEncontrado.Key, out _);
            }
            catch
            {
                return false;
            }
        }
    }
}
