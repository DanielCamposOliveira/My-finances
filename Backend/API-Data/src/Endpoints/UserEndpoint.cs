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
            // ROTAS: CRIA USUARIO
            // ==========================================
            var Endpoint = app.MapGroup("/api/v1/user");

            Endpoint.MapPost("/auth/register", async ([FromBody] RegisterRequest dto, IUserService service) =>
            {
                var response = await service.RegisterUserAsync(dto);
                return response;
            })
            .WithSummary("Register")
            .WithTags("authentication")
            .WithDescription("Registra um novo usuário")
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<TagResponseDto>(StatusCodes.Status204NoContent)
            .RequireRateLimiting("IpLimitPolicy");


            // ==========================================
            // ROTAS: LOGIN USUARIO
            // ==========================================
            Endpoint.MapPost("/auth/sign-in", async ([FromBody] LoginRequest dto, IUserService service) =>
            {
                var response = await service.AuthenticationUserAsync(dto);
                return response;
            })
            .WithSummary("login")
            .WithTags("authentication")
            .WithDescription("Autentica o usuário e retorna um token JWT")
            .Produces(StatusCodes.Status400BadRequest)
            .Produces<TagResponseDto>(StatusCodes.Status200OK)
            .RequireRateLimiting("IpLimitPolicy");


            // ==========================================
            // ROTAS: DELETAR USUARIO
            // ==========================================
            Endpoint.MapPost("/{UserDelete}", async (string UserDelete, IUserService service, ClaimsPrincipal userClaims) =>
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
            .Produces<TagResponseDto>(StatusCodes.Status201Created)
            .RequireAuthorization().RequireRateLimiting("IpLimitPolicy");

        }
    }
}
