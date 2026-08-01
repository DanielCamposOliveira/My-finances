using API_Data.src.DTOs.Lancamento;
using API_Data.src.Enum;
using API_Data.src.Services;

namespace API_Data.src.Endpoints
{
    public static class LancamentoEndpoint
    {
        public static void MapLancamentoEndpoints(this IEndpointRouteBuilder app)
        {
            var Endpoint = app.MapGroup("/api/v1/lancamentos").WithTags("lancamentos");

            // ==========================================
            // ROTAS: CRIAR LANÇAMENTO
            // ==========================================
            Endpoint.MapPost("/", async (Create dto, LancamentosService lancamentosService) =>
            {
                var resultado = await lancamentosService.CriarLancamentoAsync(dto);
                return resultado;
            })
            .WithSummary("Criar Lancamento")
            .WithDescription("Cria Lancamentos")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);



            // ==========================================
            // ROTAS: LISTAR PARCELAS
            // ==========================================
            Endpoint.MapGet("/parcela", async (LancamentosService lancamentosService) =>
            {
                var lancamentos = await lancamentosService.ListarLancamentosAsync();
                return lancamentos;
            })
            .WithName("Lista todo os Lancamento")
            .WithSummary("Lista todo os Lancamento")
            .WithDescription("Lista todas as parcelas de todos os Lancamentos")
            .Produces<List<LancamentoResponse>>(StatusCodes.Status200OK);



            // ==========================================
            // ROTAS: LISTAR PARCELAS PENDENTES
            // ==========================================
            Endpoint.MapGet("/parcela/pendentes", async (LancamentosService lancamentosService) =>
            {
                var lancamentos = await lancamentosService.ListFaturaPendenteAsync();
                return lancamentos;
            })
            .WithName("Lista os Lancamento")
            .WithSummary("Lista os Lancamento do mes")
            .WithDescription("Lista todas as parcelas de todos os Lancamentos")
            .Produces<List<LancamentoResponse>>(StatusCodes.Status200OK);



            // ==========================================
            // ROTAS: ATUALIZAR O STATUS DA CONTA FIXA
            // ==========================================
            Endpoint.MapPatch("/parcela/update", async (ParcelaUpdateStatus dto, LancamentosService service) =>
            {
                var result = await service.UptateStatusLancamentoParcela(dto.ParcelaId, dto.Status);
                return result;
            })
            .WithSummary("Atualiza Status da parcela")
            .WithDescription("Atualiza o status da parcela")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status200OK);

        }
    }
}
