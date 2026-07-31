
using API_Data.src.Data;
using API_Data.src.Endpoints;
using API_Data.src.Repository;
using API_Data.src.Services;
using Microsoft.EntityFrameworkCore;
using static API_Data.src.DTOs.LancamentoDto;

var builder = WebApplication.CreateBuilder(args);


// Recupera a string de conexão do appsettings.json de Conexão com o PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");

// Configura a Injeção de Dependência para o EF Core usar o PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// Registro de Serviços de Repositório e Regra de Negócio no Container de DI
builder.Services.AddScoped<LancamentosRepository>();
builder.Services.AddScoped<LancamentosService>();
builder.Services.AddScoped<ContasFixasRepository>();
builder.Services.AddScoped<ContasFixasService>();
builder.Services.AddScoped<TagRepository>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<CategoriaRepository>();
builder.Services.AddScoped<CategoriaService>();

// serviços necessários para o Swagger funcionar
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();


app.UseSwagger();

// Configurações do Swagger no ambiente de Desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Info Host API v1");
        // Se quiser que o Swagger abra digitando apenas http://localhost:8585/, deixe a linha abaixo.
        // Se preferir acessar por http://localhost:8585/swagger, comente a linha abaixo com //
        c.RoutePrefix = string.Empty;
    });
}

// Mapeamento dos grupos de endpoints
app.MapContasFixasEndpoints();
app.MapParcelasEndpoints();
app.MapTagEndpoints();
app.MapCategoriaEndpoints();








#region lancamentos

// ==========================================
// ROTAS: LANÇAMENTOS (Com regras de negócio)
// ==========================================
var lancamentosGroup = app.MapGroup("/api/lancamentos").WithTags("Lançamentos");

lancamentosGroup.MapPost("/", async (CriarLancamentoDto dto, LancamentosService lancamentosService) =>
{
    try
    {
        var resultado = await lancamentosService.CriarLancamentoAsync(dto);
        return Results.Created($"/api/lancamentos/{resultado.Id}", resultado);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { mensagem = ex.Message });
    }
})
.WithName("CriarLancamento")
.Produces<LancamentoResponseDto>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

lancamentosGroup.MapGet("/", async (LancamentosService lancamentosService) =>
{
    var lancamentos = await lancamentosService.ObterTodosLancamentosAsync();
    return Results.Ok(lancamentos);
})
.WithName("ObterLancamentos")
.Produces<List<LancamentoResponseDto>>(StatusCodes.Status200OK);

#endregion





app.Run();
