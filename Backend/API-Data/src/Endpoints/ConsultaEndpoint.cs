using API_Data.src.Services.Interface;
using System.Security.Claims;

namespace API_Data.src.Endpoints
{
    public static class ConsultaEndpoint
    {
        public static void MapConsultaEndpoints(this IEndpointRouteBuilder app)
        {                    
 
            var EndpointBase = app.MapGroup("/api/v1/Consulta").WithTags("Consulta");
            var EndpointRead = EndpointBase.MapGroup("").RequireAuthorization().RequireRateLimiting("UserReadPolicy");
         


            // - Dashboard
            // ==========================================
            // ROTAS: VALOR TOTAL DE CONTAS A RECEBER
            // ==========================================
            EndpointRead.MapGet("/Valores/receber", async (IConsultaService service, ClaimsPrincipal userClaims) =>
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
            .Produces<decimal>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

            // - Dashboard
            // ==========================================
            // ROTAS: VALOR TOTAL DE SALDO
            // ==========================================
            EndpointRead.MapGet("/Valores/saldo", async (IConsultaService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var soma = await service.TotalReceitas(userId);
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das Receitas")
            .WithDescription("Retorna a soma das Receitas recebido mês e ano")
            .Produces<decimal>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);



            // - Consulta interna
            // ==========================================
            // ROTAS: VALOR TOTAL DE DIVIDAS PAGAS E REALIZADAS NO MÊS
            // ==========================================
            EndpointRead.MapGet("/Dividas/Quitadas", async (IConsultaService service, ClaimsPrincipal userClaims) =>
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
            .Produces<decimal>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

            // - Consulta interna
            // ==========================================
            // ROTAS: VALOR TOTAL DE DIVIDAS REALIZADAS NO MÊS
            // ==========================================
            EndpointRead.MapGet("/Dividas/Mes", async (IConsultaService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var soma = await service.TotalDespesas(userId);
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das Despesas do Mês")
            .WithDescription("Retorna o valor total das todas Despesas que foram criadas no mês, independente de estarem pagas ou não")
            .Produces<decimal>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);


            // - Dashboard
            // ==========================================
            // ROTAS: VALOR TOTAL DE DIVIDAS PENDENTES
            // ==========================================
            EndpointRead.MapGet("/Dividas/pendentes", async (IConsultaService service, ClaimsPrincipal userClaims) =>
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
            .Produces<decimal>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);


            // - Dashboard
            // ==========================================
            // ROTAS: VALOR TOTAL DE DIVIDAS PENDENTES
            // ==========================================
            EndpointRead.MapGet("/Dividas/total", async (IConsultaService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var soma = await service.TotalContasMesFull(userId);
                return Results.Ok(soma);
            })
            .WithSummary("Obter soma das Contas Pendentes")
            .WithDescription("Retorna o valor total das todas dividas que foram criadas no mês que esta em aberto e as contas Atrasado dos meses anteriores")
            .Produces<decimal>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        }
    }
}
