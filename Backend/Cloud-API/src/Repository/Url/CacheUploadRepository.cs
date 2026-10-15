using Cloud_API.src.Model;
using Microsoft.Extensions.Caching.Memory;

namespace Cloud_API
{
    public class CacheUploadRepository
    {
        private readonly IMemoryCache _cache;// Cache em memória para armazenar os dados de URL


        // Lista para rastrear quais chaves nós criamos
        private readonly HashSet<string> _keys = new();

        public CacheUploadRepository(IMemoryCache cache )
        {
            _cache = cache;
                    
        }

        // Método para adicionar um item ao cache com expiração
        // UploadToken é a chave de registro
        public void Add(string UploadToken, UrlUploadModel data)
        {
            // Configuramos a expiração absoluta para o item, usando a data de expiração do objeto
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = data.ExpiresAt
            };

            // Quando o item expirar, removemos ele da nossa lista de chaves automaticamente
            options.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                lock (_keys) { _keys.Remove(key.ToString()); }
            });

            // Adicionamos o item ao cache
            _cache.Set(UploadToken, data, options);

            // Adicionamos a chave à nossa lista de chaves
            lock (_keys) { _keys.Add(UploadToken); }
        }


        public void Remove(string token)
        {
            _cache.Remove(token);

            // Opcional: remover da lista de chaves manualmente para manter o GetAll limpo imediatamente
            lock (_keys)
            {
                _keys.Remove(token);
            }
        }


        // Método para obter um item do cache
        public UrlUploadModel? Get(string token)
        {
            return _cache.Get<UrlUploadModel>(token);
        }

        // Método para obter um item do cache, retornando null se não existir
        public UrlUploadModel? GetSingle(string token)
        {
            if (_cache.TryGetValue(token, out UrlUploadModel data))
            {
                return data;
            }

            return null;
        }

        // Método para obter todos os itens do cache
        public List<UrlUploadModel> GetAll()
        {
            // Criamos uma lista para armazenar os itens encontrados
            var allEntries = new List<UrlUploadModel>();

            lock (_keys)
            {
                foreach (var key in _keys)
                {
                    // Tentamos obter o item do cache usando a chave
                    if (_cache.TryGetValue(key, out UrlUploadModel item))
                    {
                        // Se o item for encontrado, adicionamos ele à lista de resultados
                        allEntries.Add(item);
                    }
                }
            }

            // Retornamos a lista de itens encontrados
            return allEntries;
        }

    }
}
