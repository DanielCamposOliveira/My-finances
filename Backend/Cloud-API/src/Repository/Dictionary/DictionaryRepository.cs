using Cloud_API.src.Model;
using System.Collections.Concurrent;

namespace Cloud_API.src.Repository.Dictionary
{
    public class DictionaryRepository
    {
        // dicionário para armazenar os dados da Tarefa, onde a chave é o TokenVideoBase        
        public readonly ConcurrentDictionary<string, Documento> BancoDadosMemoria = new(); 
    }
}
