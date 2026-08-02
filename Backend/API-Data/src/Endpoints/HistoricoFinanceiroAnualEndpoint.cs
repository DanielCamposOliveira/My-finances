using API_Data.src.DTOs;
using API_Data.src.Services.Interface;

namespace API_Data.src.Endpoints
{
    public static class HistoricoFinanceiroAnualEndpoint
    {
        public static void MapHistoricoFinanceiroAnualEndpoints(this IEndpointRouteBuilder app)
        {
            var Endpoint = app.MapGroup("/api/v1/HistoricoFinanceiroAnual").WithTags("Historico Financeiro Anual");

            // ==========================================
            // ROTAS:LISTAR HISTORICO FINANCEIRO ANUAL
            // ==========================================

            Endpoint.MapGet("/{ano:int}", async (int ano, IHistoricoFinanceiroAnualService service) =>
            {
                var result = await service.ListaHistoricoAsync(ano);
                return result;
            })
            .WithSummary("Listar Histórico Financeiro Anual")
            .WithDescription("Retorna o histórico financeiro anual para um ano específico")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);

            // ==========================================
            // ROTAS:ATUALIZAR HISTORICO FINANCEIRO ANUAL
            // ==========================================

            Endpoint.MapPost("/AtualizarHistoricoMes", async (HistoricoMesRequest request, IHistoricoFinanceiroAnualService service) =>
            {
                var result = await service.AtualizarHistoricoMesAsync(request);
                return result;
            }) .WithSummary("Atualizar Histórico Financeiro Anual")
               .WithDescription("Atualiza o histórico financeiro anual para um mês específico")
               .Produces(StatusCodes.Status200OK)
               .Produces(StatusCodes.Status400BadRequest)
               .Produces(StatusCodes.Status500InternalServerError);

        }

    }
}
