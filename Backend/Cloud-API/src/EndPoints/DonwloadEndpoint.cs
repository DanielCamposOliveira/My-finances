using Cloud_API.src.Service.Download;

namespace Cloud_API.src.EndPoints
{
    public static class DonwloadEndpoint
    {
        public static void MapDonwloadEndpoints(this IEndpointRouteBuilder app)
        {
            var taskGroupUpload = app.MapGroup(StreamsRoutes.BaseArchive).WithTags("Download");


            taskGroupUpload.MapGet(StreamsRoutes.DownloadBase.Base, async (string token,  IDownloadService service) =>
            {
                var response = await service.DownloadBase(token);
                return response;
            })
            .WithSummary("Download PDF")
            .WithDescription("Download do documento")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);
        }

    }
}
