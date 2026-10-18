using API_Data.src.Services.Interface;
using System.Security.Claims;

namespace API_Data.src.Endpoints
{
    public static class ArquivosEndpoint
    {
        public static void MapArquivosEndpoints(this IEndpointRouteBuilder app)
        {
            // ==========================================
            // ROTAS: ARQUIVOS
            // ========================================== 
            var EndpointBase = app.MapGroup("/api/v1/Arquivos").WithTags("Arquivos");
            var EndpointDownload = EndpointBase.MapGroup("").RequireAuthorization().RequireRateLimiting("UserReadPolicy");
            var EndpointUpload = EndpointBase.MapGroup("").RequireAuthorization().RequireRateLimiting("UserReadPolicy");


            EndpointUpload.MapGet("/upload", async (IArquivosService service, ClaimsPrincipal userClaims) =>
            {
                // Recupera o ID do usuário logado a partir das claims do token JWT
                var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Se não houver ID de usuário, retorna 401 Unauthorized
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();


                var resultado = await service.Upload();
                return resultado;

            })
            .WithSummary("Realiza o download/busca da URL do arquivo via token")
            .WithDescription("Consome a API externa de output passando o token e repassando o token JWT de autenticação")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status429TooManyRequests);
        }
    }
}
