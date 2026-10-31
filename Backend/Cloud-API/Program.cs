using Cloud_API;
using Cloud_API.src.Authentication;
using Cloud_API.src.Background.BackgroundLocal;
using Cloud_API.src.Background.BackgroundProcess;
using Cloud_API.src.EndPoints;
using Cloud_API.src.Process.Download;
using Cloud_API.src.Process.Storage;
using Cloud_API.src.Process.Upload;
using Cloud_API.src.Repository.Chunk;
using Cloud_API.src.Repository.DataBase;
using Cloud_API.src.Repository.Dictionary;
using Cloud_API.src.Service.Consulta;
using Cloud_API.src.Service.Download;
using Cloud_API.src.Service.Storage;
using Cloud_API.src.Service.Upload;
using Cloud_API.src.Service.Url;
using Cloud_API.src.Utils;
using HashidsNet;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("Servidor rodando em ");

// Configura a Injeção de Dependência para o EF Core usar o SQLite
builder.Services.AddDbContext<AppDbContext>();

// Adiciona o serviço de cache em memória nativo do .NET
builder.Services.AddMemoryCache();
builder.Services.AddAntiforgery();

// ============================================================
// 1. CONFIGURAÇÃO DE AUTENTICAÇÃO NATIVA (API KEY)
// ============================================================
builder.Services.AddAuthentication("ApiKeyScheme")
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKeyScheme", null);

builder.Services.AddAuthorization();

// ============================================================
// INJEÇÃO DE DEPENDÊNCIA
// ============================================================
builder.Services.AddHostedService<TaskBackground>();
builder.Services.AddHostedService<BackgroundServiceLocal>();

builder.Services.AddSingleton<Hashids>(provider =>
    new Hashids(builder.Configuration.GetSection("config:SaltSecret").Value, 7));
builder.Services.AddSingleton<SnowflakeGenerator>();
builder.Services.AddSingleton<CacheDownloadRepository>();
builder.Services.AddSingleton<CacheUploadRepository>();
builder.Services.AddSingleton<DictionaryRepository>();
builder.Services.AddSingleton<ChunkRepository>();

builder.Services.AddSingleton<IFilaDeTrabalho, FilaDeTrabalho>();

builder.Services.AddScoped<IUrlService, UrlService>();

builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IUploadProcess, UploadProcess>();

builder.Services.AddScoped<IDownloadService, DownloadService>();
builder.Services.AddScoped<IDownloadProcess, DownloadProcess>();

builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IStorageProcess, StorageProcess>();

builder.Services.AddScoped<IConsultaService, ConsultaService>();

builder.Services.AddScoped<IDataBaseRepository, DataBaseRepository>();

builder.Services.AddTransient<IGeneratorName, GeneratorName>();

// ============================================================
// SWAGGER
// ============================================================
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Cloud - API",
        Version = "v1"
    });

    // Define como o Swagger deve descrever a autenticação por Key
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Insira sua chave API no cabeçalho. Nome: X-Api-Key",
        In = ParameterLocation.Header,
        Name = "X-Api-Key",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    // Define que todas as rotas (exceto as do Swagger) exigem a autenticação por Key
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configura o IIS/Kestrel para permitir uploads de ficheiros grandes
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue;
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DesenvolvimentoTotal", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ============================================================
// CONSTRÓI A APLICAÇÃO
// ============================================================
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// ============================================================
// SWAGGER MIDDLEWARE
// ============================================================
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API-Data v1");
    c.RoutePrefix = string.Empty; // Interface abre diretamente na raiz: http://localhost:<porta>/
});

// ============================================================
// MIDDLEWARE PIPELINE
// ============================================================
app.UseCors("DesenvolvimentoTotal");

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// ============================================================
// ENDPOINTS
// ============================================================
app.MapUploadEndpoints();
app.MapDonwloadEndpoints();
app.MapUrlEndpoints();
app.MapStorageEndpoint();
app.MapConsultaEndpoints();

app.Run();