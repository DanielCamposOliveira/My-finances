using API_Data.src.Data;
using API_Data.src.DTOs;
using API_Data.src.Enum;
using API_Data.src.Model;
using API_Data.src.Repository;
using API_Data.src.Services;
using Microsoft.EntityFrameworkCore;
using static API_Data.src.DTOs.CategoriaDTO;
using static API_Data.src.DTOs.LancamentoDto;
using static API_Data.src.DTOs.TagDTO;

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


app.MapGet("/", () => "Hello World!");

// ==========================================
// ROTAS: CATEGORIAS
// ==========================================
var categoriasGroup = app.MapGroup("/api/categorias").WithTags("Categorias");

categoriasGroup.MapGet("/", async (AppDbContext db) =>
{
    var categorias = await db.Categorias
        .Select(c => new CategoriaResponseDto(c.Id, c.Nome, c.Atribuicao))
        .ToListAsync();

    return Results.Ok(categorias);
})
.WithName("ObterCategorias")
.Produces<List<CategoriaResponseDto>>(StatusCodes.Status200OK);

categoriasGroup.MapPost("/", async (CriarCategoriaDto dto, AppDbContext db) =>
{
    var categoria = new Categoria
    {
        Nome = dto.Nome,
        Atribuicao = dto.Atribuicao
    };

    db.Categorias.Add(categoria);
    await db.SaveChangesAsync();

    var response = new CategoriaResponseDto(categoria.Id, categoria.Nome, categoria.Atribuicao);
    return Results.Created($"/api/categorias/{categoria.Id}", response);
})
.WithName("CriarCategoria")
.Produces<CategoriaResponseDto>(StatusCodes.Status201Created);


// ==========================================
// ROTAS: TAGS
// ==========================================
var tagsGroup = app.MapGroup("/api/tags").WithTags("Tags");

tagsGroup.MapGet("/", async (AppDbContext db) =>
{
    var tags = await db.Tags
        .Select(t => new TagResponseDto(t.Id, t.Nome))
        .ToListAsync();

    return Results.Ok(tags);
})
.WithName("ObterTags")
.Produces<List<TagResponseDto>>(StatusCodes.Status200OK);

tagsGroup.MapPost("/", async (CriarTagDto dto, AppDbContext db) =>
{
    var tag = new Tag { Nome = dto.Nome };
    db.Tags.Add(tag);
    await db.SaveChangesAsync();

    var response = new TagResponseDto(tag.Id, tag.Nome);
    return Results.Created($"/api/tags/{tag.Id}", response);
})
.WithName("CriarTag")
.Produces<TagResponseDto>(StatusCodes.Status201Created);


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










var contasFixasGroup = app.MapGroup("/api/contas-fixas").WithTags("Contas Fixas");

contasFixasGroup.MapPost("/", async (CriarContaFixaDto dto, ContasFixasService service) =>
{
    try
    {
        var resultado = await service.CriarContaFixaAsync(dto);
        return Results.Created($"/api/contas-fixas/{resultado.Id}", resultado);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { mensagem = ex.Message });
    }
})
.WithName("CriarContaFixa")
.Produces<ContaFixaResponseDto>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

contasFixasGroup.MapGet("/faturas/{ano:int}/{mes:int}", async (int ano, int mes, ContasFixasService service) =>
{
    var faturas = await service.ObterOuGerarFaturasDoMesAsync(ano, mes);
    return Results.Ok(faturas);
})
.WithName("ObterFaturasDoMes")
.Produces<List<FaturaMesResponseDto>>(StatusCodes.Status200OK);








app.Run();
