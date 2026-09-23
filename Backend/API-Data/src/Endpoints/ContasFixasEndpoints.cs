using API_Data.src.DTOs.ContasFixas;
using API_Data.src.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_Data.src.Endpoints
{
    public static class ContasFixasEndpoints
    {
        public static void MapContasFixasEndpoints(this IEndpointRouteBuilder app)
        {
           
            var EndpointBase = app.MapGroup("/api/v1/ContasFixas").WithTags("Contas Fixas");
            var EndpointRead = EndpointBase.MapGroup("").RequireAuthorization().RequireRateLimiting("UserReadPolicy");
            var EndpointWrite = EndpointBase.MapGroup("").RequireAuthorization().RequireRateLimiting("UserWritePolicy");

            // ==========================================
            // ROTAS:CRIA CONTA
            // ==========================================
            EndpointWrite.MapPost("/", async ([FromBody] Create dto, IContasFixasService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var result = await service.CriarContaFixaAsync(dto, userId);
                return result;
            })
            .WithSummary("Cria Conta Fixa")
            .WithDescription("Cria Conta Fixa para todos os Meses")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

            // ==========================================
            // ROTAS: GERA AS FATURAS DO MÊS
            // ==========================================
            EndpointWrite.MapPost("/generator", async (IContasFixasService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var result = await service.GerarFaturasMesAsync(userId);
                return result;
            })
            .WithSummary("Gera Faturas do Mês")
            .WithDescription("Gera as faturas para o mês atual")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);


            // ==========================================
            // ROTAS: LISTAR PARCELAS PENDENTES
            // ==========================================
            EndpointRead.MapGet("/", async (IContasFixasService lancamentosService, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var lancamentos = await lancamentosService.ListaTodasContasFixa(userId);
                return lancamentos;
            })
            .WithName("Lista Todas as Contas Fixa")
            .WithSummary("Lista Todas as contas fixas")
            .WithDescription("Lista todas as parcelas de todos os Lancamentos")
            .Produces<List<ParcelasResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);


            // ==========================================
            // ROTAS: LISTAR PARCELAS PENDENTES
            // ==========================================
            EndpointRead.MapGet("/parcela/pendentes", async (IContasFixasService lancamentosService, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var lancamentos = await lancamentosService.ListFaturaPendenteAsync(userId);
                return lancamentos;
            })
            .WithName("Lista Parcelas das Contas Fixa pendentes")
            .WithSummary("Lista as parcelas pendentes das contas fixas")
            .WithDescription("Lista todas as parcelas de todos os Lancamentos")
            .Produces<List<ParcelasResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);



            // ==========================================
            // ROTAS: ATUALIZAR O STATUS DA CONTA FIXA
            // ==========================================
            EndpointWrite.MapPatch("/update/status", async ([FromBody] ContaFixaUpdateStatus dto, IContasFixasService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var result = await service.UpdateStatusContaFixa(dto, userId);
                return Results.Ok(result);
            })
            .WithSummary("Atualiza Status conta fixa")
            .WithDescription("Atualiza o status da conta fixa")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);


            // ==========================================
            // ROTAS: ATUALIZAR O STATUS DA CONTA FIXA
            // ==========================================
            EndpointWrite.MapPatch("/parcela/update/status", async ([FromBody] ParcelaUpdateStatus dto, IContasFixasService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var result = await service.UpdateStatusParcela(dto, userId);
                return Results.Ok(result);
            })
            .WithSummary("Atualiza Status parcela conta fixa")
            .WithDescription("Atualiza o status da parcela conta fixa")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);


            // ==========================================
            // ROTAS: ATUALIZAR O VALOR DA CONTA FIXA
            // ==========================================
            EndpointWrite.MapPatch("/parcela/update/valor", async ([FromBody] ParcelaUpdateValor dto, IContasFixasService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var result = await service.UpdateValorParcela(dto, userId);
                return Results.Ok(result);
            })
            .WithSummary("Atualiza Valor parcela conta fixa")
            .WithDescription("Atualiza o valor da parcela conta fixa")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);

        }
    }
}