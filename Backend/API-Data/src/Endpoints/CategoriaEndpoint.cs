using API_Data.src.DTOs;
using API_Data.src.Services;


namespace API_Data.src.Endpoints
{
    public static class CategoriaEndpoint
    {
        public static void MapCategoriaEndpoints(this IEndpointRouteBuilder app)
        {
            var Endpoint = app.MapGroup("/api/categorias").WithTags("Categorias");

            // ==========================================
            // ROTAS: CRIA Categoria
            // ==========================================


            Endpoint.MapPost("/", async (CriarCategoriaDto dto, CategoriaService service) =>
            {
                var response = await service.CriarCategoria(dto);
                return response;
            })
            .WithSummary("Categoria Add")
            .WithDescription("Criar Categoria")
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<CategoriaResponseDto>(StatusCodes.Status201Created);


            // ==========================================
            // ROTAS: LISTA Categoria
            // ==========================================
            Endpoint.MapGet("/", async (CategoriaService service) =>
            {
                var response = await service.ListaCategoria();
                return response;
            })
            .WithSummary("List Categoria")
            .WithDescription("Lista as Categoria")
            .Produces(StatusCodes.Status500InternalServerError)
            .Produces<CategoriaResponseDto>(StatusCodes.Status201Created);

        }
    }
}
