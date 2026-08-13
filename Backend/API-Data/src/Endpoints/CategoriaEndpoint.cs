using API_Data.src.DTOs;
using API_Data.src.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace API_Data.src.Endpoints
{
    public static class CategoriaEndpoint
    {
        public static void MapCategoriaEndpoints(this IEndpointRouteBuilder app)
        {
            var Endpoint = app.MapGroup("/api/v1/categorias").WithTags("Categorias");

            // ==========================================
            // ROTAS: CRIA Categoria
            // ==========================================


            Endpoint.MapPost("/", async ([FromBody] CriarCategoriaDto dto, ICategoriaService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var response = await service.CriarCategoria(dto, userId);
                return response;
            })
            .WithSummary("Categoria Add")
            .WithDescription("Criar Categoria")
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<CategoriaResponseDto>(StatusCodes.Status201Created)
            .RequireAuthorization().RequireRateLimiting("IpLimitPolicy");


            // ==========================================
            // ROTAS: LISTA Categoria
            // ==========================================
            Endpoint.MapGet("/", async (ICategoriaService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var response = await service.ListaCategoria(userId);
                return response;
            })
            .WithSummary("List Categoria")
            .WithDescription("Lista as Categoria")
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<CategoriaResponseDto>(StatusCodes.Status201Created)
            .RequireAuthorization().RequireRateLimiting("IpLimitPolicy");

        }
    }
}
