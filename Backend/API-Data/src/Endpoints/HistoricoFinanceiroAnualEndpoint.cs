using API_Data.src.DTOs;
using API_Data.src.Services.Interface;
using System.Security.Claims;

namespace API_Data.src.Endpoints
{
    public static class HistoricoFinanceiroAnualEndpoint
    {
        public static void MapHistoricoFinanceiroAnualEndpoints(this IEndpointRouteBuilder app)
        {
            var EndpointBase = app.MapGroup("/api/v1/HistoricoFinanceiroAnual").WithTags("Historico Financeiro Anual");
            var EndpointRead = EndpointBase.MapGroup("").RequireAuthorization().RequireRateLimiting("UserReadPolicy");
            var EndpointWrite = EndpointBase.MapGroup("").RequireAuthorization().RequireRateLimiting("UserWritePolicy");

            // ==========================================
            // ROTAS:LISTAR HISTORICO FINANCEIRO ANUAL
            // ==========================================

            EndpointRead.MapGet("/{ano:int}", async (int ano, IHistoricoFinanceiroAnualService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var result = await service.ListaHistoricoAsync(ano, userId);
                return result;
            })
            .WithSummary("Listar Histórico Financeiro Anual")
            .WithDescription("Retorna o histórico financeiro anual para um ano específico")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

            // ==========================================
            // ROTAS:ATUALIZAR HISTORICO FINANCEIRO ANUAL
            // ==========================================

            EndpointWrite.MapPatch("/AtualizarHistoricoMes", async (HistoricoMesRequest request, IHistoricoFinanceiroAnualService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var result = await service.UpdateHistoricoMesAsync(request, userId);
                return result;
            }) .WithSummary("Atualizar Histórico Financeiro Anual")
               .WithDescription("Atualiza o histórico financeiro anual para um mês específico")
               .Produces(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status400BadRequest)
               .Produces(StatusCodes.Status500InternalServerError)
               .Produces(StatusCodes.Status401Unauthorized)
               .Produces(StatusCodes.Status429TooManyRequests);



            // ==========================================
            // ROTAS:GERAR HISTORICO FINANCEIRO MENSAL
            // ==========================================
            EndpointWrite.MapPost("/generator", async (IHistoricoFinanceiroAnualService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var result = await service.GerarHistoricoMesAsync(userId);
                return result;
            })
            .WithSummary("Gerar Histórico Financeiro Anual")
            .WithDescription("Gera o histórico financeiro anual para um ano específico")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        }

    }
}
