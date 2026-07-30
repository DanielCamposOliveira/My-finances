
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

#region  ROTAS: CATEGORIAS

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

#endregion


#region ROTAS: TAGS

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

#endregion


#region Descricao

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


#region Contas Fixas


var contasFixasGroup = app.MapGroup("/api/v1/ContasFixas").WithTags("Contas Fixas");

// ==========================================
// ROTAS:CRIA CONTA
// ==========================================
contasFixasGroup.MapPost("/create", async (CriarContaFixaDto dto, ContasFixasService service) =>
{
    try
    {
        var result = await service.CriarContaFixaAsync(dto);
        return result;
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { mensagem = ex.Message });
    }
})
.WithSummary("Cria Conta Fixa")
.WithDescription("Cria Conta Fixa para todos os Meses")
.Produces(StatusCodes.Status201Created)
.Produces(StatusCodes.Status500InternalServerError);

// ==========================================
// ROTAS: ATUALIZAR O STATUS DA CONTA FIXA
// ==========================================
contasFixasGroup.MapPatch("/status", async (ContaFixaUpdateDTO.ContaFixaUpdateStatusDTO dto, ContasFixasService service) =>
{
    var result = await service.UpdateStatusContaFixa(dto.Id_ContaFixa, dto.Status);
    return Results.Ok(result);
})
.WithSummary("Atualiza Status conta fixa")
.WithDescription("Atualiza o status da conta fixa")
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError)
.Produces(StatusCodes.Status200OK);


// ==========================================
// ROTAS:CRIA A PARCELA DA FATURAS DO MES
// ==========================================
contasFixasGroup.MapPost("/fatura/create", async (ContasFixasService service) =>
{
    var result = await service.GerarFaturasMesAsync();
    return result;
})
.WithSummary("Add fatura")
.WithDescription("Criar fatura do Mes Atual")
.Produces(StatusCodes.Status201Created)
.Produces(StatusCodes.Status200OK);


// ==========================================
// ROTAS: LISTA AS FATURAS EM ABERTO DO MES E AS ATRAZADAS
// ==========================================
contasFixasGroup.MapGet("/fatura/pending", async (ContasFixasService service) =>
{
    var result = await service.ListFaturaPendenteAsync();
    return Results.Ok(result);
})
.WithSummary("Lista faturas")
.WithDescription("Lista todas as faturas em ABERTO mes atual e ATRAZADAS ")
.Produces<List<FaturaMesResponseDto>>(StatusCodes.Status200OK);


// ==========================================
// ROTAS: ATUALIZAR O STATUS DA FATURAS
// ==========================================
contasFixasGroup.MapPatch("/fatura/status", async (FaturaUpdateDTO.FaturaUpdateStatusDTO dto, ContasFixasService service) =>
{
    var result = await service.UpdateStatusParcela(dto.ParcelaId, dto.Status);
    return Results.Ok(result);
})
.WithSummary("Atualiza Status faturas")
.WithDescription("Atualiza o status da faturas")
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError)
.Produces(StatusCodes.Status200OK);





#endregion


app.Run();
