using API_Data.src.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_Data.src.Utils
{
    public class JwtService : IJwtService
    {
        private readonly byte[] _keyBytes;

        public JwtService(IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"]
                ?? throw new Exception("Chave JWT não configurada.");

            _keyBytes = Encoding.UTF8.GetBytes(key);
        }
        /// <summary>
        /// Gera um token JWT (JSON Web Token) para o usuário informado.
        /// </summary>
        /// <param name="user">
        /// Usuário para o qual o token será gerado.
        /// </param>
        /// <returns>
        /// Retorna o token JWT assinado, contendo o identificador do usuário
        /// e uma data de expiração de 8 horas.
        /// </returns>
        public string GenerateToken(User user)
        {
            // Cria o manipulador responsável por gerar e serializar o token JWT.
            var tokenHandler = new JwtSecurityTokenHandler();

            // Define as configurações do token, incluindo as claims,
            // tempo de expiração e as credenciais utilizadas para assinatura.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Adiciona o ID do usuário como uma claim de identificação.
                // Essa informação pode ser utilizada para identificar o usuário autenticado.
                Subject = new ClaimsIdentity(new[]
                {       
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                }),

                // Define a expiração do token para 8 horas a partir do horário atual em UTC.
                Expires = DateTime.UtcNow.AddHours(8),

                // Define a chave e o algoritmo utilizados para assinar digitalmente o token.
                // A assinatura permite verificar se o token foi alterado após sua geração.
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_keyBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            // Cria o token JWT com base nas configurações definidas acima.
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Converte o token para o formato de string utilizado nas requisições HTTP.
            return tokenHandler.WriteToken(token);
        }

    }
}
