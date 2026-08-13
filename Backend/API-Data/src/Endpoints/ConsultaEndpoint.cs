using API_Data.src.Services.Interface;
using System.Security.Claims;

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
            EndpointValores.MapGet("/receber", async (IConsultaService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var soma = await service.TotalReceber(userId);
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das Contas a Receber")
            .WithDescription("Retorna a soma das contas a receber para um determinado mês e ano")
            .Produces<decimal>(StatusCodes.Status200OK);

            // - Dashboard
            // ==========================================
            // ROTAS: VALOR TOTAL DE SALDO
            // ==========================================
            EndpointValores.MapGet("/saldo", async (IConsultaService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var soma = await service.TotalSaldo(userId);
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das Contas em Saldo")
            .WithDescription("Retorna a soma das contas em saldo para um determinado mês e ano")
            .Produces<decimal>(StatusCodes.Status200OK);




            // - Consulta interna
            // ==========================================
            // ROTAS: VALOR TOTAL DE DIVIDAS PAGAS E REALIZADAS NO MÊS
            // ==========================================
            EndpointDividas.MapGet("/Quitadas", async (IConsultaService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var soma = await service.TotalQuitadasDoMes(userId);
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das Contas Quitadas do Mês")
            .WithDescription("Retorna a soma das contas quitadas do mês para um determinado mês e ano")
            .Produces<decimal>(StatusCodes.Status200OK);

            // - Consulta interna
            // ==========================================
            // ROTAS: VALOR TOTAL DE DIVIDAS REALIZADAS NO MÊS
            // ==========================================
            EndpointDividas.MapGet("/Mes", async (IConsultaService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var soma = await service.TotalDividasMes(userId);
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das Contas Dividas do Mês")
            .WithDescription("Retorna o valor total das todas dividas que foram criadas no mês, independente de estarem pagas ou não")
            .Produces<decimal>(StatusCodes.Status200OK);


            // - Dashboard
            // ==========================================
            // ROTAS: VALOR TOTAL DE DIVIDAS PENDENTES
            // ==========================================
            EndpointDividas.MapGet("/pendentes", async (IConsultaService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var soma = await service.TotalContasPendentes(userId);
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das Contas Pendentes")
            .WithDescription("Retorna o valor total das todas dividas que foram criadas no mês que esta em aberto e as contas Atrasado dos meses anteriores")
            .Produces<decimal>(StatusCodes.Status200OK);

        }
    }
}
