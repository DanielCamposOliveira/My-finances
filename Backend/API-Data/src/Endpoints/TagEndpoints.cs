using API_Data.src.DTOs;
using API_Data.src.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_Data.src.Endpoints
{
    public static class TagEndpoints
    {
        public static void MapTagEndpoints(this IEndpointRouteBuilder app)
        {
            // ==========================================
            // ROTAS: CRIA TAGS
            // ==========================================            
            var EndpointBase = app.MapGroup("/api/v1/tags").WithTags("Tags");
            var EndpointRead = EndpointBase.MapGroup("").RequireAuthorization().RequireRateLimiting("UserReadPolicy");
            var EndpointWrite = EndpointBase.MapGroup("").RequireAuthorization().RequireRateLimiting("UserWritePolicy");

            EndpointRead.MapPost("/", async ([FromBody]  CriarTagDto dto, ITagService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var response = await service.CriarTag(dto, userId);
                return response;
            })
            .WithSummary("Tag Add")
            .WithDescription("Criar Tag")
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<TagResponseDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);


            // ==========================================
            // ROTAS: LISTA TAGS
            // ==========================================
            EndpointWrite.MapGet("/", async (ITagService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var response = await service.ListaTags(userId);
                return response;
            })
            .WithSummary("Tag List")
            .WithDescription("Lista Tag")
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<TagResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);
        }
    }
}
