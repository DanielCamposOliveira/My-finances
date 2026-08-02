using API_Data.src.DTOs;
using API_Data.src.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace API_Data.src.Endpoints
{
    public static class TagEndpoints
    {
        public static void MapTagEndpoints(this IEndpointRouteBuilder app)
        {
            // ==========================================
            // ROTAS: CRIA TAGS
            // ==========================================
            var Endpoint = app.MapGroup("/api/v1/tags").WithTags("Tags");

            Endpoint.MapPost("/", async ([FromBody]  CriarTagDto dto, ITagService service) =>
            {
                var response = await service.CriarTag(dto);
                return response;
            })
            .WithSummary("Tag Add")
            .WithDescription("Criar Tag")
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<TagResponseDto>(StatusCodes.Status201Created);


            // ==========================================
            // ROTAS: LISTA TAGS
            // ==========================================
            Endpoint.MapGet("/", async (ITagService service) =>
            {
                var response = await service.ListaTags();
                return response;
            })
            .WithSummary("Tag List")
            .WithDescription("Lista Tag")
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<TagResponseDto>(StatusCodes.Status201Created);

        }
    }
}
