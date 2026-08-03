using API_Data.src.Services.Interface;

namespace API_Data.src.Endpoints
{
    public static class ConsultaEndpoint
    {
        public static void MapConsultaEndpoints(this IEndpointRouteBuilder app)
        {
                    
            var EndpointValores = app.MapGroup("/api/v1/Consulta/Valores").WithTags("Consulta");
            var EndpointDividas = app.MapGroup("/api/v1/Consulta/Dividas").WithTags("Consulta");

            // - Dashboard
            // ==========================================
            // ROTAS: VALOR TOTAL DE CONTAS A RECEBER
            // ==========================================
            EndpointValores.MapGet("/receber", async (IConsultaService service) =>
            {
                var soma = await service.TotalReceber();
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das Contas a Receber")
            .WithDescription("Retorna a soma das contas a receber para um determinado mês e ano")
            .Produces<decimal>(StatusCodes.Status200OK);

            // - Dashboard
            // ==========================================
            // ROTAS: VALOR TOTAL DE SALDO
            // ==========================================
            EndpointValores.MapGet("/saldo", async (IConsultaService service) =>
            {
                var soma = await service.TotalSaldo();
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das Contas em Saldo")
            .WithDescription("Retorna a soma das contas em saldo para um determinado mês e ano")
            .Produces<decimal>(StatusCodes.Status200OK);




            // - Consulta interna
            // ==========================================
            // ROTAS: VALOR TOTAL DE DIVIDAS PAGAS E REALIZADAS NO MÊS
            // ==========================================
            EndpointDividas.MapGet("/Quitadas", async (IConsultaService service) =>
            {
                var soma = await service.TotalQuitadasDoMes();
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das Contas Quitadas do Mês")
            .WithDescription("Retorna a soma das contas quitadas do mês para um determinado mês e ano")
            .Produces<decimal>(StatusCodes.Status200OK);

            // - Consulta interna
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


            // - Dashboard
            // ==========================================
            // ROTAS: VALOR TOTAL DE DIVIDAS PENDENTES
            // ==========================================
            EndpointDividas.MapGet("/pendentes", async (IConsultaService service) =>
            {
                var soma = await service.TotalContasPendentes();
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das Contas Pendentes")
            .WithDescription("Retorna o valor total das todas dividas que foram criadas no mês que esta em aberto e as contas Atrasado dos meses anteriores")
            .Produces<decimal>(StatusCodes.Status200OK);

        }
    }
}
