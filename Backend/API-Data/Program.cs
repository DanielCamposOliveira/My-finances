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
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<
    IHistoricoFinanceiroAnualRepository,
    HistoricoFinanceiroAnualRepository>();

builder.Services.AddScoped<
    IHistoricoFinanceiroAnualService,
    HistoricoFinanceiroAnualService>();

builder.Services.AddScoped<IConsultaRepository, ConsultaRepository>();
builder.Services.AddScoped<IConsultaService, ConsultaService>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IJwtService, JwtService>();


// ============================================================
// AUTENTICAÇÃO JWT
// ============================================================

var jwtKey = builder.Configuration["JWT:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT:Key não foi configurada no appsettings.json.");
}

builder.Services.AddJwtAuthentication(jwtKey);


// ============================================================
// AUTORIZAÇÃO
// ============================================================

builder.Services.AddAuthorization();


// ============================================================
// CORS
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


// ============================================================
// RATE LIMITING
// ============================================================

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("IpLimitPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey:
                httpContext.Connection.RemoteIpAddress?.ToString()
                ?? "unknown",

            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 20,
                Window = TimeSpan.FromSeconds(10),
                QueueLimit = 0
            }));
});


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