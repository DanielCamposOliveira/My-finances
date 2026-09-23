using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API_Data.src.Extensions
{
    /// <summary>
    /// Extensão para configurar a autenticação JWT no ASP.NET Core.
    /// </summary>
    /// 

    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            string secretKey)
        {
            // Validação preventiva: O algoritmo HMAC-SHA256 exige chaves com pelo menos 256 bits (32 bytes)
            if (string.IsNullOrWhiteSpace(secretKey) || Encoding.UTF8.GetByteCount(secretKey) < 32)
            {
                throw new ArgumentException(
                    "A chave JWT (secretKey) deve conter no mínimo 32 caracteres (256 bits) para garantir segurança criptográfica adequada.",
                    nameof(secretKey));
            }


            var keyBytes = Encoding.UTF8.GetBytes(secretKey);

            services.AddAuthentication(options =>
            {
                // Define o esquema padrão como Bearer para autenticação e desafios (challenges)
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Exige transporte seguro (HTTPS). Em produção deve permanecer true para evitar interceptação de tokens (Man-in-the-Middle)
                options.RequireHttpsMetadata = true;

                // Não persiste o token bruto no AuthenticationProperties para economizar memória e evitar exposição desnecessária
                options.SaveToken = true;

                // Parâmetros rígidos de validação criptográfica do token
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // 1. Assinatura e Criptografia
                    ValidateIssuerSigningKey = true, // Obriga a verificação da assinatura contra a nossa chave secreta
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes), // Chave simétrica usada para assinar e conferir o hash HMAC
                    RequireSignedTokens = true, // Rejeita qualquer tentativa de token sem assinatura (previne bypass com alg: "none")


                    // 2. Tempo e Expiração
                    ValidateLifetime = true, // Rejeita tokens com data de expiração ultrapassada
                    RequireExpirationTime = true, // Rejeita tokens que não possuam o claim 'exp' definido
                    ClockSkew = TimeSpan.Zero, // Elimina a tolerância padrão de 5 minutos, expirando o token exatamente no segundo previsto

                    // 3. Emissor (Issuer) e Destinatário (Audience)
                    // Mantenha false se sua API não gera esses campos no token, ou altere para true caso adicione Issuer/Audience no appsettings
                    ValidateIssuer = false,
                    ValidateAudience = false,                   
                };

                // Manipulação de eventos do ciclo de vida da autenticação
                options.Events = new JwtBearerEvents
                {
                    // Registra se o token expirou sem expor detalhes sensíveis
                    OnAuthenticationFailed = context =>
                    {
                        //Console.WriteLine(
                        //    $"[JWT] Falha na validação do Token: {context.Exception.Message}"
                        //);

                        // Registra se o token expirou sem expor detalhes sensíveis
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.Headers.Append("Token-Expired", "true");
                        }

                        return Task.CompletedTask;
                    },


                    OnChallenge = context =>
                    {
                        // Impede o processamento padrão do ASP.NET Core para podermos customizar a resposta
                        context.HandleResponse();

                        // Evita tentar escrever na resposta se ela já tiver sido despachada por outro middleware
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            return context.Response.WriteAsJsonAsync(new
                            {
                                status = StatusCodes.Status401Unauthorized,
                                message = "Não autorizado. Token de acesso ausente, inválido ou expirado."
                            });
                        }


                        //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        //context.Response.ContentType = "application/json";

                        //return context.Response.WriteAsJsonAsync(new
                        //{
                        //    message = "Não autorizado. Você precisa enviar um token JWT válido no Header."
                        //});

                        return Task.CompletedTask;
                    },

                    OnForbidden = context =>
                    {
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";

                            return context.Response.WriteAsJsonAsync(new
                            {
                                status = StatusCodes.Status403Forbidden,
                                message = "Acesso negado. Seu perfil não possui permissão para executar esta ação."
                            });
                        }

                        return Task.CompletedTask;
                    }
                    //OnForbidden = context =>
                    //{
                    //    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    //    context.Response.ContentType = "application/json";

                    //    return context.Response.WriteAsJsonAsync(new
                    //    {
                    //        message = "Você não tem permissão para acessar este recurso."
                    //    });
                    //},

                    //OnMessageReceived = context =>
                    //{
                    //    var authorizationHeader = context.Request.Headers["Authorization"].ToString();

                    //    Console.WriteLine($"[JWT] Token recebido no Header: {authorizationHeader}");

                    //    return Task.CompletedTask;
                    //},

                    //OnTokenValidated = context =>
                    //{
                    //    Console.WriteLine("[JWT] Token validado com sucesso!");

                    //    return Task.CompletedTask;
                    //},


                };
            });

            return services;
        }
    }

}
