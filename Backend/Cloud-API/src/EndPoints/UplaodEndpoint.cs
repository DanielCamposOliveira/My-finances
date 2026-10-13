using Cloud_API.src.Model.Upload;
using Cloud_API.src.Service.Upload;
using Microsoft.AspNetCore.Mvc;

namespace Cloud_API.src.EndPoints
{
    public static class UplaodEndpoint
    {
        public static void MapUploadEndpoints(this IEndpointRouteBuilder app)
        {
            var taskGroupUpload = app.MapGroup(StreamsRoutes.BaseMedia).WithTags("Upload");

           
            taskGroupUpload.MapPost(StreamsRoutes.UploadBase.Base, async (string token, [FromForm] UploadChunkRequestModel request, HttpContext context, IUploadService service) =>
            {
                var response = await service.UploadBase(token, request, context);
                return response;
            })
            .WithSummary("Upload PDF")
            .WithDescription("Upload Chunk")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
        }
    }
}
