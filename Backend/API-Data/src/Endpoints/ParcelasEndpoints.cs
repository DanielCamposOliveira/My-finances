using API_Data.src.DTOs;
using API_Data.src.Services;

namespace API_Data.src.Endpoints
{
    public static class ParcelasEndpoints
    {
        public static void MapParcelasEndpoints(this IEndpointRouteBuilder app)
        {
            var Endpoint = app.MapGroup("/api/v1/Parcelas").WithTags("Parcelas");

            // ==========================================
            // ROTAS:CRIA A PARCELA DA FATURAS DO MES
            // ==========================================
            Endpoint.MapPost("/generator", async (ContasFixasService service) =>
            {
                var result = await service.GerarFaturasMesAsync();
                return result;
            })
            .WithSummary("Add fatura")
            .WithDescription("Criar Parcelas do Mes Atual das Contas Fixa")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status200OK);


            // ==========================================
            // ROTAS: LISTA AS FATURAS EM ABERTO DO MES E AS ATRAZADAS
            // ==========================================
            Endpoint.MapGet("/pending", async (ContasFixasService service) =>
            {
                var result = await service.ListFaturaPendenteAsync();
                return result;
            })
            .WithSummary("Lista faturas")
            .WithDescription("Lista todas as faturas em ABERTO mes atual e ATRAZADAS ")
            .Produces<List<ParcelasContaFixaResponseDTO>>(StatusCodes.Status200OK);


            // ==========================================
            // ROTAS: ATUALIZAR O STATUS DA FATURAS
            // ==========================================
            Endpoint.MapPatch("/status", async (ParcelaUpdateStatusDTO dto, ContasFixasService service) =>
            {
                var result = await service.UpdateStatusParcela(dto.ParcelaId, dto.Status);
                return result;
            })
            .WithSummary("Atualiza Status faturas")
            .WithDescription("Atualiza o status da faturas")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status200OK);


            // ==========================================
            // ROTAS: ATUALIZAR O STATUS DA FATURAS
            // ==========================================
            Endpoint.MapPatch("/Valor", async (ParcelaUpdateValorDTO dto, ContasFixasService service) =>
            {
                var result = await service.UpdateValorParcela(dto.ParcelaId, dto.ValorParcela);
                return result;
            })
            .WithSummary("Atualiza Valor da faturas")
            .WithDescription("Atualiza o valor da faturas")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status200OK);

        }
    }
}
