
using API_Data.src.DTOs.Result;
using API_Data.src.Model;
using API_Data.src.Repository.Interface;
using API_Data.src.Services.Interface;
using API_Data.src.Utils;
using static API_Data.src.DTOs.UserDtos;

namespace API_Data.src.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public UserService(IUserRepository userRepository, IJwtService jwtService)
        {
            _jwtService = jwtService;
            _userRepository = userRepository;
        }

        private async Task<OperationResult> IsAdminUser(string userId)
        {
            // verifica se o Id esta vazio
            if (string.IsNullOrEmpty(userId))
                return new OperationResult
                {
                    Success = false,
                    Message = "userId não informado"
                };

            // Busca usuario
            var User = await _userRepository.GetUserByIdAsync(userId);


            if (User == null)
                return new OperationResult
                {
                    Success = false,
                    Message = "Usuario não encontrado"
                };

            if (User.IsActive == false)
                return new OperationResult
                {
                    Success = false,
                    Message = "Usuario Desativado"
                };

            if (User.IsAdmin == false)
                return new OperationResult
                {
                    Success = false,
                    Message = "Usuario sem permissão"
                };

            return new OperationResult
            {
                Success = true,
                Message = ""
            };

        }

        private async Task<OperationResult> IsActiveUser(string userId)
        {
            // verifica se o Id esta vazio
            if (string.IsNullOrEmpty(userId))
                return new OperationResult
                {
                    Success = false,
                    Message = "userId não informado"
                };

            // Busca usuario
            var User = await _userRepository.GetUserByIdAsync(userId);


            if (User == null)
                return new OperationResult
                {
                    Success = false,
                    Message = "Usuario não encontrado"
                };

            if (User.IsActive == false)
                return new OperationResult
                {
                    Success = false,
                    Message = "Usuario Desativado"
                };

            return new OperationResult
            {
                Success = true,
                Message = ""
            };

        }

        public async Task<IResult> DeactivateUserAsync(string userId, string UserActiver)
        {
            // Verifica se o usuario esta Ativo
            var resultUser = await IsAdminUser(userId);
            if (!resultUser.Success)
                return Results.BadRequest(new { message = resultUser.Message });


            // Tentar desativar a URL usando o repositório
            var result = await _userRepository.DeactivateUserAsync(UserActiver);

            // Verificar se a operação foi bem-sucedida
            if (!result.Success)
                return Results.BadRequest(new { message = result.Message });

            return Results.Ok(new { message = result.Message });
        }

        public async Task<IResult> DeleteUser(string userId, string UserDelete)
        {
            var resultUser = await IsAdminUser(userId);
            if (!resultUser.Success)
                return Results.BadRequest(new { message = resultUser.Message });

            var result = await _userRepository.DeleteUserAsync(UserDelete);

            if (!result.Success)
                return Results.BadRequest(new { message = resultUser.Message });

            return Results.NoContent(); 
        }

        public async Task<IResult> GetUserInfo(string userId)
        {
            // Busca usuario
            var User = await _userRepository.GetUserByIdAsync(userId);

            // Verifica se o usuário foi encontrado
            if (User == null)
            {
                return Results.NotFound();
            }

            var dados = new UserInfo
            {
                Name = User.Name,
                IsActive = User.IsActive,
                IsAdmin = User.IsAdmin,     
            };

            return Results.Ok(dados);
        }

        public async Task<IResult> ObterPageUserAsync(string userId, int page, int limit)
        {
            var resultUser = await IsAdminUser(userId);
            if (!resultUser.Success)
                return Results.BadRequest(new{ Message = resultUser.Message });


            // Garantir que a página seja pelo menos 1
            if (page < 1) page = 1;
            // Garantir que o limite esteja entre 1 e 50, caso contrário, definir para 10
            if (limit < 1 || limit > 50) limit = 10;

            // Obter a lista de URLs paginadas para o usuário especificado
            var data = await _userRepository.GetUserPageAsync(userId, page, limit);

            return Results.Ok(data);
        }
        
        public async Task<IResult> PostAuthenticationUserAsync(LoginRequest req)
        {
            // Validar o email
            if (string.IsNullOrWhiteSpace(req.Email))
                return Results.BadRequest(new { message = "Email inválido." });

            // Consultar o repositório para obter o usuário correspondente ao email
            var user = await _userRepository.GetUserByEmailAsync(req.Email);

            // Se o usuário não for encontrado, retornar null
            if (user == null)
                return Results.BadRequest(new { message = "Usuário não encontrado." });

            // Verificar a senha usando o PasswordHasher
            if (!PasswordHasher.VerifyPassword(req.Password, user.PasswordHash))
                return Results.BadRequest(new { message = "Senha incorreta." });

            // Verifica se o usuario esta Ativo
            var result = await IsActiveUser(user.Id);
            if (!result.Success)
                return Results.BadRequest(new { message = result.Message });


            // Gerar o token JWT usando o IJwtService
            var token = _jwtService.GenerateToken(user);

            // Retornar a resposta de autenticação com o token
            return Results.Ok(new AuthResponse(token));
        }

        public async Task<IResult> PostRegisterUserAsync(RegisterRequest User)
        {
            // Registrar o usuário no repositório
            var result = await _userRepository.RegisterUserAsync(User);

            // Verificar se o registro foi bem-sucedido
            if (!result.Success)
                return Results.BadRequest(new { message = result.Message });

            return Results.Created(); // 201 Created
        }

    
    }
}
