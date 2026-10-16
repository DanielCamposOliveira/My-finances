using Cloud_API.src.Service.Consulta;

namespace Cloud_API.src.EndPoints
{
    public static class ConsultarEndpont
    {
        public static void MapConsultaEndpoints(this IEndpointRouteBuilder app)
        {
            var taskGroupConsulta = app.MapGroup(StreamsRoutes.BaseConsultae).WithTags("Search");


            taskGroupConsulta.MapGet(StreamsRoutes.Consulta.Base, async (string token, HttpContext context, IConsultaService service) =>
            {
                var response = await service.ObterRegistro(token);
                return response;
            })
            .WithSummary("Search PDF")
            .WithDescription("Listar documento")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();


            taskGroupConsulta.MapGet(StreamsRoutes.Consulta.BaseNoToken, async ( HttpContext context, IConsultaService service) =>
            {
                var response = await service.ObterTodosRegistro();
                return response;
            })
            .WithSummary("Lista PDF")
            .WithDescription("Listar Todos documento")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
        }
    }
}
