using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Cloud_API.src.Authentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string APIKEYNAME = "X-Api-Key";
        private readonly IConfiguration _configuration;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IConfiguration configuration) : base(options, logger, encoder)
        {
            _configuration = configuration;
        }



        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // 1. Verifica se o header existe
            if (!Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                return AuthenticateResult.Fail("API Key não fornecida.");
            }

            // 2. Valida nas configurações
            var validApiKeys = _configuration.GetSection("ApiKeys").GetChildren().Select(s => s.Value).ToList();

            if (validApiKeys == null || !validApiKeys.Contains(extractedApiKey))
            {
                return AuthenticateResult.Fail("Acesso não autorizado.");
            }

            // 3. Se for válido, cria a identidade da requisição
            var claims = new[] { new Claim(ClaimTypes.Name, "ApiKeyUser") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
