using Cloud_API.src.Service.Url;
using static Cloud_API.StreamsRoutes;

namespace Cloud_API.src.EndPoints
{
    public static class UrlEndPoint
    {
        public static void MapUrlEndpoints(this IEndpointRouteBuilder app)
        {
            var taskGroupUpload = app.MapGroup(UrlBase.Upload).WithTags("URL");

            var taskGroupDownload = app.MapGroup(UrlBase.Donwload).WithTags("URL");


            // Rota acessada pela API usando Key
            // Gera uma URL para o usuario fazer o UPload do PDF sem autenticação 
            taskGroupUpload.MapGet("/", async (HttpContext context, IUrlService service) =>
            {
                var response = await service.UrlGenerateUpload(context);
                return response;
            })
            .WithSummary("URL Upload")
            .WithDescription("Obtem URL")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

            // Rota acessada pela API usando Key
            // Gera uma URL para o usuario fazer o Download do PDF sem autenticação 
            taskGroupDownload.MapGet("/", async (string token, HttpContext context, IUrlService service) =>
            {
                var response = await service.UrlGenerateDownload(token, context);
                return response;
            })
            .WithSummary("URL Download")
            .WithDescription("Obtem URL")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();


        }
    }
}
