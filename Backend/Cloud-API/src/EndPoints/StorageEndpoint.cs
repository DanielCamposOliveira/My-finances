using Cloud_API.src.Service.Storage;
using static Cloud_API.StreamsRoutes;

namespace Cloud_API.src.EndPoints
{
    public static class StorageEndpoint
    {
        public static void MapStorageEndpoint(this IEndpointRouteBuilder app)
        {
            var taskGroupDelete = app.MapGroup(DeleteBase.Base).WithTags("Delete");

            // Rota acessada pela API usando Key
            // Gera uma URL para o usuario fazer o UPload do PDF sem autenticação 
            taskGroupDelete.MapDelete("/", async (string token,  IStorageService service) =>
            {
                var response = await service.Delete(token);
                return response;
            })
            .WithSummary("Deleta PDF")
            .WithDescription("Deleta documento pdf")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
        }
    }
}
