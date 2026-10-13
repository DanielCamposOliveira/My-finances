using Microsoft.Extensions.Caching.Memory;

namespace Cloud_API
{
    public class UrlDownloadRepository
    {
        // Cache em memória para armazenar os dados de URL
        private readonly IMemoryCache _cache;

        // Lista para rastrear quais chaves nós criamos
        private readonly HashSet<string> _keys = new();

        public UrlDownloadRepository(IMemoryCache cache)
        {
            _cache = cache;
        }


        // Método para adicionar um item ao cache com expiração
        public void Add(UrlDownload data)
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
            _cache.Set(data.Token, data, options);

            // Adicionamos a chave à nossa lista de chaves
            lock (_keys) { _keys.Add(data.Token); }
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
        public UrlDownload? Get(string token)
        {
            return _cache.Get<UrlDownload>(token);
        }

        // Método para obter um item do cache, retornando null se não existir
        public UrlDownload? GetSingle(string token)
        {
            if (_cache.TryGetValue(token, out UrlDownload data))
            {
                return data;
            }

            return null;
        }

        // Método para obter todos os itens do cache
        public List<UrlDownload> GetAll()
        {
            // Criamos uma lista para armazenar os itens encontrados
            var allEntries = new List<UrlDownload>();

            lock (_keys)
            {
                foreach (var key in _keys)
                {
                    // Tentamos obter o item do cache usando a chave
                    if (_cache.TryGetValue(key, out UrlDownload item))
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
