using Cloud_API;
using Cloud_API.src.Authentication;
using Cloud_API.src.EndPoints;
using Cloud_API.src.Process.Download;
using Cloud_API.src.Process.Upload;
using Cloud_API.src.Repository.Chunk;
using Cloud_API.src.Repository.Dictionary;
using Cloud_API.src.Service.Download;
using Cloud_API.src.Service.Upload;
using Cloud_API.src.Service.Url;
using Cloud_API.src.Utils;
using HashidsNet;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);



// Adiciona o serviço de cache em memória nativo do .NET
builder.Services.AddMemoryCache();

builder.Services.AddAntiforgery();

// ============================================================
// 1. CONFIGURAÇÃO DE AUTENTICAÇÃO NATIVA (API KEY)
// ============================================================

builder.Services.AddAuthentication("ApiKeyScheme")
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKeyScheme", null);

builder.Services.AddAuthorization(); // Habilita o suporte a [Authorize] / RequireAuthorization()



// ============================================================
// INJEÇÃO DE DEPENDÊNCIA
// ============================================================
builder.Services.AddSingleton<UrlDownloadRepository>();
builder.Services.AddSingleton<UrlUploadRepository>();
builder.Services.AddSingleton<DictionaryRepository>();
builder.Services.AddSingleton<ChunkRepository>();



builder.Services.AddScoped<IUrlService, UrlService>();
builder.Services.AddScoped<IUploadService, UploadService>();

builder.Services.AddScoped<DownloadService>();

builder.Services.AddScoped<UploadProcess>();

builder.Services.AddScoped<DownloadProcess>();

//------------- verificar ----------------
builder.Services.AddScoped<IDownloadProcess, DownloadProcess>();

// 1. Registre o SnowflakeGenerator como Singleton (precisa ser Singleton para manter o estado da sequência)
builder.Services.AddSingleton<SnowflakeGenerator>();

// 2. Registre o Hashids
builder.Services.AddSingleton<Hashids>(provider => new Hashids(builder.Configuration.GetSection("config:SaltSecret").Value, 7));

// 3. Registre a sua classe GeneratorName (como Transient ou Scoped, dependendo da sua preferência)
builder.Services.AddTransient<GeneratorName>();






// ============================================================
// SWAGGER
// ============================================================
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    // Define como o Swagger deve descrever a autenticação por Key
    c.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Insira sua chave API no cabeçalho. Nome: access_token",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "access_token",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });
    
    // Define que todas as rotas (exceto as do Swagger) exigem a autenticação por Key
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new string[] {}
        }
    });

});

// Configura o IIS para permitir uploads de arquivos grandes, removendo o limite de tamanho do corpo da requisição
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue;
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DesenvolvimentoTotal", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
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





// ============================================================
// SWAGGER
// ============================================================

app.UseSwagger();
app.UseSwaggerUI();

// Configura o Swagger UI para usar a definição de API v1
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Info Host API v1"); // Garante que o endpoint do Swagger UI aponte para a sua definição v1
    c.RoutePrefix = string.Empty;
});





// ============================================================
// MIDDLEWARE
// ============================================================
// 1º -  ForwardedHeaders e RateLimiter (executa antes do CORS e Autenticação)
// A ordem importa: Autenticação vem antes da Autorização

app.UseCors("DesenvolvimentoTotal"); // Deve ficar entre o UseRouting e o UseAuthorization

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();





// ============================================================
// ENDPOINTS
// ============================================================
app.MapUploadEndpoints();
app.MapDonwloadEndpoints();
app.MapUrlEndpoints();



app.Run();






