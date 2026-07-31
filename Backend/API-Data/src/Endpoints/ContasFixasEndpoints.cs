using API_Data.src.DTOs;
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
            Endpoint.MapPost("/create", async (ContaFixaCreateDTO dto, ContasFixasService service) =>
            {
                var result = await service.CriarContaFixaAsync(dto);
                return result;
            })
            .WithSummary("Cria Conta Fixa")
            .WithDescription("Cria Conta Fixa para todos os Meses")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status500InternalServerError);

            // ==========================================
            // ROTAS: ATUALIZAR O STATUS DA CONTA FIXA
            // ==========================================
            Endpoint.MapPatch("/status", async (ContaFixaUpdateStatusDTO dto, ContasFixasService service) =>
            {
                var result = await service.UpdateStatusContaFixa(dto.Id_ContaFixa, dto.Status);
                return Results.Ok(result);
            })
            .WithSummary("Atualiza Status conta fixa")
            .WithDescription("Atualiza o status da conta fixa")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status200OK);


            

        }
    }
}
