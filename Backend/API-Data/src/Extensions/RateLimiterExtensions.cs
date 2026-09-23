using Microsoft.AspNetCore.HttpOverrides;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace API_Data.src.Extensions
{

    public static class RateLimiterExtensions
    {
        /// <summary>
        /// Configura os serviços de Forwarded Headers e Rate Limiter particionado por IP.
        /// </summary>
        public static IServiceCollection AddRateLimitingConfiguration(this IServiceCollection services)
        {
            // 1. Configuração do Forwarded Headers nos Serviços
            // Quando a API roda atrás de um proxy reverso (Nginx, Cloudflare, Traefik,AWS ALB ou Docker),
            // O ASP.NET Core enxerga apenas o IP do próprio proxy na conexão
            // TCP direta. Sem essa configuração, todos os clientes da internet compartilham o mesmo
            // IP no httpContext.Connection.RemoteIpAddress
            // Resolvemos o problema causado pelo Item 2. AddRateLimiter
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                //X-Forwarded-For: contém o endereço IP original do cliente.
                //X-Forwarded-Proto: contém o protocolo original utilizado pelo cliente http ou https
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                // Limpa as redes/proxies padrão caso esteja rodando em Docker, Kubernetes ou atrás de Nginx/Cloudflare
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            // 2. Configuração do Rate Limiter por IP
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // =========================================================================
                // POLÍTICA Por IP (Rotas públicas como Login, Registro, Recuperação)
                // =========================================================================
                options.AddPolicy("IpLimitPolicy", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 10,
                            Window = TimeSpan.FromSeconds(420), // 7 min
                            QueueLimit = 0 // Permite segurar requisições na fila ate libera o limite
                        }));

                // =========================================================================
                // POLÍTICA: Por Usuário Logado - Leituras autenticadas (GET / Consultas gerais)
                // =========================================================================    
                   
                options.AddPolicy("UserReadPolicy", httpContext =>
                {
                    var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? httpContext.User.FindFirst("sub")?.Value
                              ?? httpContext.Connection.RemoteIpAddress?.ToString()
                              ?? "unknown";

                    return RateLimitPartition.GetTokenBucketLimiter(
                        partitionKey: userId,
                        factory: partition => new TokenBucketRateLimiterOptions
                        {
                            // 1. TAMANHO DO BALDE (Anti-rajada)   
                            // Permite uma rajada de até 30 requisições quando o balde está cheio.
                            // A partir da 31ª, as requisições são rejeitadas até que novos tokens sejam repostos.
                            TokenLimit = 20, // balde\Bucket comporta no máximo 30 unidades e o tamanho da rajada que o usuário pode fazer

                            // 2. REPOSIÇÃO CONTÍNUA (Fluxo sustentado)    
                            // FORMULA
                            // 60segundos / ReplenishmentPeriod(5) = 12 ciclo
                            // ciclo(12) * TokensPerPeriod(30) = 360 requisições por minutos
                            // Pode usar 6 requisições por segundos ou 30 requisições diluido nos 5 segundos

                            //Obs. algoritmo do Token Bucket no .NET calcula a reposição de forma proporcional ao tempo decorrido, em vez de esperar os 10 segundos inteiros para jogar 5 fichas de uma vez só.
                            // Entao GetTokenBucketLimiter vaz a diluição das requisições TokensPerPeriod / ReplenishmentPeriod
                            // Jà o GetFixedWindowLimiter trava o usuario ate o fim do siclo ReplenishmentPeriod

                            TokensPerPeriod = 5,  //quantidade de requisições que volta para balde. Repõe 30 tokens a cada ReplenishmentPeriod(5)segundos
                            ReplenishmentPeriod = TimeSpan.FromSeconds(5),  // intervalo 5 segundos para reposição.
                            AutoReplenishment = true,       // Repõe os tokens automaticamente a cada período definido em ReplenishmentPeriod.
                            QueueLimit = 0      // quantas requisições podem esperar em vez de serem rejeitadas. 0 = Rejeita na hora sem segurar em fila
                        });
                });


                // =========================================================================
                // POLÍTICA: Por Usuário Logado - Escritas críticas (POST, PUT, DELETE / Pagamentos / Lançamentos)
                // =========================================================================
                options.AddPolicy("UserWritePolicy", httpContext =>
                {
                    var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? httpContext.User.FindFirst("sub")?.Value
                              ?? httpContext.Connection.RemoteIpAddress?.ToString()
                              ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: userId,
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 15,                  // Apenas 15 operações de escrita
                            Window = TimeSpan.FromSeconds(60), // por minuto
                            QueueLimit = 0                     // Rejeita na hora sem enfileirar
                        });
                });


            });

            return services;
        }

        /// <summary>
        /// Registra os middlewares de Forwarded Headers e Rate Limiter na ordem correta do pipeline.
        /// </summary>
        public static IApplicationBuilder UseRateLimitingConfiguration(this IApplicationBuilder app)
        {
            // 1. O UseForwardedHeaders DEVE vir antes do UseRateLimiter para resolver o IP real
            app.UseForwardedHeaders();

            // 2. Aplica o middleware de Rate Limiting
            app.UseRateLimiter();

            return app;
        }
    }
}
