using API_Data.src.DTOs;
using API_Data.src.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static API_Data.src.DTOs.UserDtos;

namespace API_Data.src.Endpoints
{
    public static class UserEndpoint
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            // ==========================================
            // ROTAS: REGISTRO
            // ==========================================
            // Grupo base comum (compartilha o prefixo e a documentação Swagger)
            var EndpointBase = app.MapGroup("/api/v1/user").WithTags("user");

            // Subgrupo 1: Rotas públicas (Login, Registro) -> Limitadas por IP
            var EndpointPublic= EndpointBase.MapGroup("").RequireRateLimiting("IpLimitPolicy");
            // Subgrupo 2: Rotas protegidas (Deletar, Perfil, Atualizar) -> Limitadas por Usuário
            var EndpointProtected = EndpointBase.MapGroup("").RequireAuthorization().RequireRateLimiting("UserLimitPolicy");


            EndpointPublic.MapPost("/auth/register", async ([FromBody] RegisterRequest dto, IUserService service) =>
            {
                var response = await service.RegisterUserAsync(dto);
                return response;
            })
            .WithSummary("Register")
            .WithTags("authentication")
            .WithDescription("Registra um novo usuário")
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<TagResponseDto>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status500InternalServerError);


            // ==========================================
            // ROTAS: LOGIN USUARIO
            // ==========================================
            EndpointPublic.MapPost("/auth/sign-in", async ([FromBody] LoginRequest dto, IUserService service) =>
            {
                var response = await service.AuthenticationUserAsync(dto);
                return response;
            })
            .WithSummary("login")
            .WithTags("authentication")
            .WithDescription("Autentica o usuário e retorna um token JWT")
            .Produces(StatusCodes.Status400BadRequest)
            .Produces<TagResponseDto>(StatusCodes.Status200OK);


            // ==========================================
            // ROTAS: DELETAR USUARIO
            // ==========================================
            EndpointProtected.MapDelete("/{UserDelete}", async (string UserDelete, IUserService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var result = await service.DeleteUser(userId, UserDelete);

                return result;
            })
            .WithSummary("DELETE USER")
            .WithTags("Administrator")
            .WithDescription("Exclui usuário")
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<TagResponseDto>(StatusCodes.Status201Created);



            // ==========================================
            // NOVA ROTA: LOGIN / REGISTRO COM GOOGLE
            // ==========================================
            EndpointPublic.MapPost("/auth/google", async ([FromBody] GoogleAuthRequest dto, IUserService service) =>
            {
                return await service.AuthenticateGoogleUserAsync(dto);
            })
            .WithSummary("Login com Google")
            .WithTags("authentication")
            .WithDescription("Autentica ou cadastra o usuário via Google ID Token e retorna o JWT da API")
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<AuthResponse>(StatusCodes.Status200OK);


        }
    }
}
