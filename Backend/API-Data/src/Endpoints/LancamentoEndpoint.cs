using API_Data.src.DTOs.Lancamento;
using API_Data.src.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_Data.src.Endpoints
{
    public static class LancamentoEndpoint
    {
        public static void MapLancamentoEndpoints(this IEndpointRouteBuilder app)
        {            
            var EndpointBase = app.MapGroup("/api/v1/lancamentos").WithTags("lancamentos");
            var EndpointRead = EndpointBase.MapGroup("").RequireAuthorization().RequireRateLimiting("UserReadPolicy");
            var EndpointWrite = EndpointBase.MapGroup("").RequireAuthorization().RequireRateLimiting("UserWritePolicy");


            // ==========================================
            // ROTAS: CRIAR LANÇAMENTO
            // ==========================================
            EndpointWrite.MapPost("/", async ([FromBody] Create dto, ILancamentosService ILancamentosService, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var resultado = await ILancamentosService.CriarLancamentoAsync(dto, userId);
                return resultado;
            })
            .WithSummary("Criar Lancamento")
            .WithDescription("Cria Lancamentos")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);




            EndpointRead.MapGet("/", async (ILancamentosService ILancamentosService, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var lancamentos = await ILancamentosService.ListarLancamentosAsync(userId);
                return lancamentos;
            })
            .WithName("Lista todo os Lancamento")
            .WithSummary("Lista todo os Lancamento")
            .WithDescription("Lista todas as parcelas de todos os Lancamentos")
            .Produces<List<LancamentoResponse>>(StatusCodes.Status200OK)
            .RequireAuthorization().RequireRateLimiting("IpLimitPolicy");


            // ==========================================
            // ROTAS: LISTAR PARCELAS
            // ==========================================
            EndpointRead.MapGet("/parcela", async (ILancamentosService ILancamentosService, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var lancamentos = await ILancamentosService.ListarLancamentosAsync(userId);
                return lancamentos;
            })
            .WithName("Lista todas as Parcelas")
            .WithSummary("Lista todas as Parcelas")
            .WithDescription("Lista todas as parcelas do usuário de forma linear")
            .Produces<List<ParcelasResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);


            // ==========================================
            // ROTAS: LISTAR PARCELAS PENDENTES
            // ==========================================
            EndpointRead.MapGet("/parcela/pendentes", async (ILancamentosService ILancamentosService, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var lancamentos = await ILancamentosService.ListFaturaPendenteAsync(userId);
                return lancamentos;
            })
            .WithName("Lista os Lancamento")
            .WithSummary("Lista os Lancamento do mes")
            .WithDescription("Lista todas as parcelas de todos os Lancamentos")
            .Produces<List<LancamentoResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);



            // ==========================================
            // ROTAS: ATUALIZAR O STATUS DA CONTA FIXA
            // ==========================================
            EndpointWrite.MapPatch("/parcela/update/status", async ([FromBody] ParcelaUpdateStatus dto, ILancamentosService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var result = await service.UptateStatusLancamentoParcela(dto, userId);
                return result;
            })
            .WithSummary("Atualiza Status da parcela")
            .WithDescription("Atualiza o status da parcela")
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);


            // ==========================================
            // ROTAS: ATUALIZAR O VALOR DA CONTA FIXA
            // ==========================================
            EndpointWrite.MapPatch("/parcela/update/valor", async ([FromBody] ParcelaUpdateValor dto, ILancamentosService service, ClaimsPrincipal userClaims) =>
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
