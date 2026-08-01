using API_Data.src.DTOs.ContasFixas;
using API_Data.src.Services;

namespace API_Data.src.Endpoints
{
    public static class ContasFixasEndpoints
    {
        public static void MapContasFixasEndpoints(this IEndpointRouteBuilder app)
        {
            var Endpoint = app.MapGroup("/api/v1/ContasFixas").WithTags("Contas Fixas");


            // ==========================================
            // ROTAS:CRIA CONTA
            // ==========================================
            Endpoint.MapPost("/", async (Create dto, ContasFixasService service) =>
            {
                var result = await service.CriarContaFixaAsync(dto);
                return result;
            })
            .WithSummary("Cria Conta Fixa")
            .WithDescription("Cria Conta Fixa para todos os Meses")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status500InternalServerError);

            // ==========================================
            // ROTAS: GERA AS FATURAS DO MÊS
            // ==========================================
            Endpoint.MapPost("/generator", async (ContasFixasService service) =>
            {
                var result = await service.GerarFaturasMesAsync();
                return result;
            })
            .WithSummary("Gera Faturas do Mês")
            .WithDescription("Gera as faturas para o mês atual")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status200OK);


            // ==========================================
            // ROTAS: LISTAR PARCELAS PENDENTES
            // ==========================================
            Endpoint.MapGet("/", async (ContasFixasService lancamentosService) =>
            {
                var lancamentos = await lancamentosService.ListaTodasContasFixa();
                return lancamentos;
            })
            .WithName("Lista Todas as Contas Fixa")
            .WithSummary("Lista Todas as contas fixas")
            .WithDescription("Lista todas as parcelas de todos os Lancamentos")
            .Produces<List<ParcelasResponse>>(StatusCodes.Status200OK);


            // ==========================================
            // ROTAS: LISTAR PARCELAS PENDENTES
            // ==========================================
            Endpoint.MapGet("/parcela/pendentes", async (ContasFixasService lancamentosService) =>
            {
                var lancamentos = await lancamentosService.ListFaturaPendenteAsync();
                return lancamentos;
            })
            .WithName("Lista Parcelas das Contas Fixa pendentes")
            .WithSummary("Lista as parcelas pendentes das contas fixas")
            .WithDescription("Lista todas as parcelas de todos os Lancamentos")
            .Produces<List<ParcelasResponse>>(StatusCodes.Status200OK);



            // ==========================================
            // ROTAS: ATUALIZAR O STATUS DA CONTA FIXA
            // ==========================================
            Endpoint.MapPatch("/update/status", async (ContaFixaUpdateStatus dto, ContasFixasService service) =>
            {
                var result = await service.UpdateStatusContaFixa(dto.Id_ContaFixa, dto.Status);
                return Results.Ok(result);
            })
            .WithSummary("Atualiza Status conta fixa")
            .WithDescription("Atualiza o status da conta fixa")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status200OK);


            // ==========================================
            // ROTAS: ATUALIZAR O STATUS DA CONTA FIXA
            // ==========================================
            Endpoint.MapPatch("/parcela/update/status", async (ParcelaUpdateStatus dto, ContasFixasService service) =>
            {
                var result = await service.UpdateStatusParcela(dto.ParcelaId, dto.Status);
                return Results.Ok(result);
            })
            .WithSummary("Atualiza Status parcela conta fixa")
            .WithDescription("Atualiza o status da parcela conta fixa")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status200OK);


            // ==========================================
            // ROTAS: ATUALIZAR O VALOR DA CONTA FIXA
            // ==========================================
            Endpoint.MapPatch("/parcela/update/valor", async (ParcelaUpdateValor dto, ContasFixasService service) =>
            {
                var result = await service.UpdateValorParcela(dto.ParcelaId, dto.ValorParcela);
                return Results.Ok(result);
            })
            .WithSummary("Atualiza Valor parcela conta fixa")
            .WithDescription("Atualiza o valor da parcela conta fixa")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status200OK);




        }
    }
}