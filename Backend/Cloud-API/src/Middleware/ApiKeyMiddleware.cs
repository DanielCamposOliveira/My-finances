namespace Cloud_API
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEYNAME = "access_token"; // Nome do Header
        private readonly IConfiguration _configuration; // Adicione esta propriedade

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration) // Injete IConfiguration
        {
            _next = next;
            _configuration = configuration; // Atribua a configuração
        }

        // Método InvokeAsync é chamado para cada requisição HTTP
        public async Task InvokeAsync(HttpContext context)
        {
            // Ignora a validação de API Key para as rotas do Swagger - para fins de produção com uso do swagger
            if (context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.StartsWithSegments("/favicon.ico"))
            {
                await _next(context);
                return;
            }

            // 1. Verifica se o header existe
            if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                // Se a chave nao for fornecida, retorna 401 Unauthorized
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key nao fornecida.");
                return;
            }

            // 2. Verifica se a chave é válida comparando com as chaves armazenadas na configuração
            var apiKeysSection = _configuration.GetSection("ApiKeys");
            var validApiKeys = apiKeysSection.GetChildren().Select(s => s.Value).ToList();

            if (validApiKeys == null || !validApiKeys.Contains(extractedApiKey))
            {
                // Se a chave fornecida nao for válida, retorna 401 Unauthorized
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Acesso nao autorizado.");
                return;
            }

            // Se a chave for válida, continua para o próximo middleware
            await _next(context);
        }
    }
}
