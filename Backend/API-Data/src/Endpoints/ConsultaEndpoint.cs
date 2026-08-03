using API_Data.src.Services.Interface;

namespace API_Data.src.Endpoints
{
    public static class ConsultaEndpoint
    {
        public static void MapConsultaEndpoints(this IEndpointRouteBuilder app)
        {
            var Endpoint = app.MapGroup("/api/v1/Consulta").WithTags("Consulta");


            

            Endpoint.MapGet("/lancamentos", async (IConsultaService service) =>
            {
                var soma = await service.ValorLancamento();
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das parcelas")
            .WithDescription("Retorna a soma das parcelas de um lançamento específico para um determinado mês e ano")
            .Produces<decimal>(StatusCodes.Status200OK);


            Endpoint.MapGet("/contasfixa", async (IConsultaService service) =>
            {
                var soma = await service.ValorContaFixa();
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das Contas Fixas")
            .WithDescription("Retorna a soma das contas fixas para um determinado mês e ano")
            .Produces<decimal>(StatusCodes.Status200OK);

            Endpoint.MapGet("/pagar", async (IConsultaService service) =>
            {
                var soma = await service.TotalContasApgar();
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das Contas a Pagar")
            .WithDescription("Retorna a soma das contas a pagar para um determinado mês e ano")
            .Produces<decimal>(StatusCodes.Status200OK);


            Endpoint.MapGet("/receber", async (IConsultaService service) =>
            {
                var soma = await service.TotalContasReceber();
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das Contas a Receber")
            .WithDescription("Retorna a soma das contas a receber para um determinado mês e ano")
            .Produces<decimal>(StatusCodes.Status200OK);

            Endpoint.MapGet("/saldo", async (IConsultaService service) =>
            {
                var soma = await service.TotalSaldo();
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das Contas em Saldo")
            .WithDescription("Retorna a soma das contas em saldo para um determinado mês e ano")
            .Produces<decimal>(StatusCodes.Status200OK);






            var EndpointDividas = app.MapGroup("/api/v1/Consulta/Dividas").WithTags("Consulta");

            // ==========================================
            // ROTAS: VALOR TOTAL DE DIVIDAS PAGAS E REALIZADAS NO MÊS
            // ==========================================
            EndpointDividas.MapGet("/Pagas", async (IConsultaService service) =>
            {
                var soma = await service.TotalPagasDoMes();
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das Contas Pagas do Mês")
            .WithDescription("Retorna a soma das contas pagas do mês para um determinado mês e ano")
            .Produces<decimal>(StatusCodes.Status200OK);



            // ==========================================
            // ROTAS: VALOR TOTAL DE DIVIDAS REALIZADAS NO MÊS
            // ==========================================
            EndpointDividas.MapGet("/Mes", async (IConsultaService service) =>
            {
                var soma = await service.TotalDividasMes();
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das Contas Dividas do Mês")
            .WithDescription("Retorna o valor total das todas dividas que foram criadas no mês, independente de estarem pagas ou não")
            .Produces<decimal>(StatusCodes.Status200OK);

        }
    }
}
