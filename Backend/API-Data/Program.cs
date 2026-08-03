
using API_Data.src.Data;
using API_Data.src.Endpoints;
using API_Data.src.Repository;
using API_Data.src.Repository.Interface;
using API_Data.src.Services;
using API_Data.src.Services.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Recupera a string de conexão do appsettings.json de Conexão com o PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");

// Configura a Injeção de Dependência para o EF Core usar o PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// Registro de Serviços de Repositório e Regra de Negócio no Container de DI
builder.Services.AddScoped<ILancamentosRepository, LancamentosRepository>();
builder.Services.AddScoped<ILancamentosService, LancamentosService>();
builder.Services.AddScoped<IContasFixasRepository, ContasFixasRepository>();
builder.Services.AddScoped<IContasFixasService, ContasFixasService>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IHistoricoFinanceiroAnualRepository, HistoricoFinanceiroAnualRepository>();
builder.Services.AddScoped<IHistoricoFinanceiroAnualService, HistoricoFinanceiroAnualService>();
builder.Services.AddScoped<IConsultaRepository, ConsultaRepository>();
builder.Services.AddScoped<IConsultaService, ConsultaService>();

// serviços necessários para o Swagger funcionar
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    // Usa o nome completo da classe (ex: API_Data.src.DTOs.Lancamento.Create) para o ID do schema
    options.CustomSchemaIds(type => type.FullName);
});

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
app.MapHistoricoFinanceiroAnualEndpoints();
app.MapTagEndpoints();
app.MapCategoriaEndpoints();
app.MapLancamentoEndpoints();
app.MapConsultaEndpoints();

app.Run();
