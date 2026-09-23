using API_Data.src.Data;
using API_Data.src.Endpoints;
using API_Data.src.Extensions;
using API_Data.src.Repository;
using API_Data.src.Repository.Interface;
using API_Data.src.Services;
using API_Data.src.Services.Interface;
using API_Data.src.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine($"Servidor rodando em {builder.Configuration["Urls:Endpoints:Https:Url"]}");

// Evita a sobreposição limpando URLs herdadas do ambiente ou padrões
//builder.WebHost.UseUrls();

// ============================================================
// BANCO DE DADOS
// ============================================================

// Recupera a string de conexão do appsettings.json de Conexão com o PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");

// Configura a Injeção de Dependência para o EF Core usar o PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));



// ============================================================
// INJEÇÃO DE DEPENDÊNCIA
// ============================================================

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

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IJwtService, JwtService>();


// ============================================================
// 1. CORS
// ============================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("Liberado", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// 1. Registro do serviço com regras recomendadas
builder.Services.AddHsts(options =>
{
    // Define o tempo que o navegador deve lembrar (padrão de mercado: 1 ano)
    options.MaxAge = TimeSpan.FromDays(365);

    // Aplica a política a todos os subdomínios (ex: api.seusite.com)
    options.IncludeSubDomains = true;

    // Permite inclusão na lista global HSTS Preload dos navegadores
    options.Preload = true;
});


// ============================================================
// 1. AUTENTICAÇÃO JWT
// ============================================================
var jwtKey = builder.Configuration["JWT:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT:Key não foi configurada no appsettings.json.");
}

builder.Services.AddJwtAuthentication(jwtKey);


// ============================================================
// 2. AUTORIZAÇÃO
// ============================================================
builder.Services.AddAuthorization();





// ============================================================
// 3. RATE LIMITING
// ============================================================
builder.Services.AddRateLimitingConfiguration();


// ============================================================
// SWAGGER
// ============================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Info Host API",
        Version = "v1"
    });

    // Evita conflito entre DTOs com o mesmo nome
    // Ex:
    // API_Data.src.DTOs.Lancamento.Create
    // API_Data.src.DTOs.ContasFixas.Create
    //options.CustomSchemaIds(type => type.FullName);
    options.CustomSchemaIds(type =>
    (type.FullName ?? type.Name).Replace("+", "."));

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});





// ============================================================
// CONSTRÓI A APLICAÇÃO
// ============================================================
var app = builder.Build();


// 2. Ativação do Middleware no pipeline
if (!app.Environment.IsDevelopment())
{
    // HSTS não funciona em localhost por padrão (e nem deve)
    app.UseHsts();
}

// Redireciona chamadas HTTP para HTTPS antes de processar as rotas
app.UseHttpsRedirection();

// ============================================================
// SWAGGER
// ============================================================
app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "Info Host API v1");

    // Swagger abre diretamente em http://localhost:xxxx/
    options.RoutePrefix = string.Empty;
});


// ============================================================
// MIDDLEWARE
// ============================================================
// 1º -  ForwardedHeaders e RateLimiter (executa antes do CORS e Autenticação)
app.UseRateLimitingConfiguration();

app.UseCors("Liberado");

app.UseAuthentication();
app.UseAuthorization();


// ============================================================
// ENDPOINTS
// ============================================================

app.MapUserEndpoints();
app.MapContasFixasEndpoints();
app.MapHistoricoFinanceiroAnualEndpoints();
app.MapTagEndpoints();
app.MapCategoriaEndpoints();
app.MapLancamentoEndpoints();
app.MapConsultaEndpoints();


app.Run();