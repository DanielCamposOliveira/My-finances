using Cloud_API;
using Cloud_API.src.Background;
using Cloud_API.src.Background.BackDataScheduler;
using Cloud_API.src.Model.Chunk;
using Cloud_API.src.Process.Download;
using Cloud_API.src.Process.Upload;
using Cloud_API.src.Service.Url;
using Cloud_API.src.Utils;
using HashidsNet;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DictionaryStorage>();

builder.Services.AddHostedService<BackgroundServiceLocal>();



// Registra os serviços no container de dependências
builder.Services.AddScoped<UploadBaseService>(); // Registra sua nova classe


builder.Services.AddScoped<DeleteDocumentService>();


builder.Services.AddScoped<IDownloadBaseService, DownloadBaseServiceMd5>();


builder.Services.AddScoped<IRoutesEndpoints, RoutesEndpoints>();
builder.Services.AddScoped<IUrlService, Cloud_API.src.Service.Url.UrlService>();

builder.Services.AddSingleton<IDataPersistenceService, SQLiteDataPersistenceService>();


//----

builder.Services.AddSingleton<IFilaDeTrabalho, FilaDeTrabalho>();
builder.Services.AddSingleton<DictionaryStorage>(); // Assumindo que este dicionário deva ser Singleton

// Registra o serviço que move o arquivo
builder.Services.AddTransient<DeleteDocumentService>();

// Ativa o monitor de fila em segundo plano
builder.Services.AddHostedService<TaskBackground>();

//----




// Adiciona o serviço de cache em memória nativo do .NET
builder.Services.AddMemoryCache();
//builder.Services.AddScoped<UrlService>();

builder.Services.AddSingleton<Cloud_API.UrlRepository>();


builder.Services.AddSingleton<UploadChunkTracker>();
builder.Services.AddHostedService<UploadCleanupService>();




// 1. Registre o SnowflakeGenerator como Singleton (precisa ser Singleton para manter o estado da sequência)
builder.Services.AddSingleton<SnowflakeGenerator>();

// 2. Registre o Hashids
builder.Services.AddSingleton<Hashids>(provider => new Hashids(builder.Configuration.GetSection("config:SaltSecret").Value, 7));

// 3. Registre a sua classe GeneratorName (como Transient ou Scoped, dependendo da sua preferência)
builder.Services.AddTransient<GeneratorName>();





// --- CONFIGURAÇÃO DE SERVIÇOS ---

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    // Define como o Swagger deve descrever a autenticação por Key
    c.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Insira sua chave API no campo abaixo. Exemplo: MinhaChave123",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "access_token", // Este é o nome do cabeçalho que o Middleware vai procurar
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

// Configura o Kestrel para permitir uploads de arquivos grandes, removendo o limite de tamanho do corpo da requisição
//builder.WebHost.ConfigureKestrel(serverOptions => {
//    serverOptions.Limits.MaxRequestBodySize = null;
//});

// Configura o IIS para permitir uploads de arquivos grandes, removendo o limite de tamanho do corpo da requisição
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue;
});


// Configura o BackDataScheduler para usar a instância do Repository e seu DictionaryDados
builder.Services.AddSingleton<BackDataScheduler>(sp =>
{
    // Obtém a instância do Repository para acessar o DictionaryDados
    var repository = sp.GetRequiredService<DictionaryStorage>();

    // Aqui você pode acessar o DictionaryDados do Repository para usar na sua ação de salvamento
    var dict = repository.DicionarioDeMemoria;

    // A ação que o BackDataScheduler vai executar periodicamente.
    // Ela tem acesso ao DictionaryDados para salvar os itens "sujos" no banco de dados.
    return new BackDataScheduler(async () => // <-- Sua `acaoExecutar`
    {


        // Itera sobre os valores do DictionaryDados para encontrar os itens "sujos" (IsDirty = true)
        // Use .Values para iterar sobre os valores do dicionário, que são os PacoteDadosModel
        foreach (var item in dict.Values) // Use .Values para iterar sobre os valores
        {
            // Verifica se o item está marcado como "sujo" (IsDirty = true)
            if (item.IsDirty) // Só salva se o item estiver marcado como "sujo"
            {
                // cria uma instancia do serviço de persistência para chamar o método de sincronização
                var dataPersistenceService = sp.GetRequiredService<IDataPersistenceService>();
                await dataPersistenceService.SincronizarBD(); // Chama o método de sincronização para salvar os dados no banco de dados

                // Após salvar, marque o item como "limpo" (IsDirty = false) para indicar que ele não precisa ser salvo novamente até que seja modificado
                item.IsDirty = false;
            }
        }

        // Se você quiser que o agendador espere somente o _intervaloExecucao
        // definido no construtor da BackDataScheduler, remova este delay:
        // await Task.Delay(30000); 
    }, TimeSpan.FromSeconds(10)); // <-- Exemplo: salva a cada 30 segundos
});


//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy.WithOrigins("http://localhost:8000") // Permite a origem do seu app MVC
//              .AllowAnyMethod()
//              .AllowAnyHeader();
//    });
//});


builder.Services.AddCors(options =>
{
    options.AddPolicy("DesenvolvimentoTotal", policy =>
    {
        policy.AllowAnyOrigin()   // Permite QUALQUER porta, IP ou domínio
              .AllowAnyHeader()   // Permite qualquer cabeçalho extra
              .AllowAnyMethod();  // Permite GET, POST, PUT, DELETE, etc.
    });
});

// ...




var app = builder.Build();

app.Logger.LogInformation("App {AppName} iniciado", app.Environment.ApplicationName);

//app.UseCors("AllowAll"); // Deve ficar entre o UseRouting e o UseAuthorization

app.UseCors("DesenvolvimentoTotal"); // Deve ficar entre o UseRouting e o UseAuthorization

app.UseSwagger();
app.UseSwaggerUI();

// Configura o Swagger UI para usar a definição de API v1
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Info Host API v1"); // Garante que o endpoint do Swagger UI aponte para a sua definição v1
    c.RoutePrefix = string.Empty;
});

// Adiciona o middleware de validação de API Key para todas as rotas, exceto as rotas do Swagger
//app.UseWhen(context => !context.Request.Path.StartsWithSegments("/swagger"), appBuilder =>
//{
//    appBuilder.UseMiddleware<ApiKeyMiddleware>();
//});


app.UseWhen(context =>
    !context.Request.Path.StartsWithSegments("/swagger") &&
    !context.Request.Path.StartsWithSegments("/media/upload/base"),
    appBuilder =>
    {
        appBuilder.UseMiddleware<ApiKeyMiddleware>();
    });


var taskGroupDev = app.MapGroup("/Dev").WithTags("Endpoints Desenvolvimento"); // Rota provisoria para carregar os dados do banco de dados para o dicionário em memória e resetar o banco de dados

// Grupos de rotas para organizar os endpoints
var taskGroupInfo = app.MapGroup(StreamsRoutes.BaseTask).WithTags("Endpoints Get");
var taskGroupDel = app.MapGroup(StreamsRoutes.BaseArchive).WithTags("Endpoints Delete");
var taskGroupMov = app.MapGroup(StreamsRoutes.BaseArchive).WithTags("Endpoints Move");
var taskGroupUpload = app.MapGroup(StreamsRoutes.BaseMedia).WithTags("Endpoints Upload");
var taskGroupDownload = app.MapGroup(StreamsRoutes.BaseMedia).WithTags("Endpoints Download");
//var downloadGroup = app.MapGroup(StreamsRoutes.BaseMedia).WithTags("Endpoints Download");
var TaskUrl = app.MapGroup(StreamsRoutes.UrlBase).WithTags("Endpoints Url");



TaskUrl.MapGet(StreamsRoutes.UrlUpload.Base, (IRoutesEndpoints service, HttpContext context ) =>
{
    var dados = service.GenerateUrl(context);
    return dados;
});




// Endpoint para obter o status de todos as Tarefas
taskGroupInfo.MapGet(StreamsRoutes.Task.AllCache, (IRoutesEndpoints service) =>
{
    var dados = service.GetAllCache();
    return dados;
});

taskGroupInfo.MapGet(StreamsRoutes.Task.FilterCache, (string token, IRoutesEndpoints service) =>
{
    var dados = service.GetSingleCache(token);
    return dados;
});







// Endpoint para obter o status do arquivo com base no token
taskGroupInfo.MapGet(StreamsRoutes.Task.Filter, (string token, IRoutesEndpoints service) =>
{
    var dados = service.GetTask(token);
    return dados;
});




;


//// Endpoint para deletar o arquivo base
//taskGroupDel.MapDelete(StreamsRoutes.Delete.Base, async (string token, IRoutesEndpoints service) =>
//{
//    return await service.DeleteTaskBase(token);
  
//});



//// Endpoint para deletar o arquivo com erro
//taskGroupDel.MapDelete(StreamsRoutes.Delete.Erro, async (string token, IRoutesEndpoints service) =>
//{
//    return await service.DeleteTaskErro(token);   
//});





// Endpoint para upload do arquivo base
taskGroupUpload.MapPost(StreamsRoutes.Upload.Base, async (string token, [FromForm] UploadChunkRequest request, HttpContext context, IRoutesEndpoints service, UploadBaseService svc) =>
{
    return await service.UploadBase(token, request, context, svc);

}).DisableAntiforgery();









taskGroupDownload.MapGet(StreamsRoutes.Download.Base, (string token, HttpContext context, IRoutesEndpoints service) =>
{
    //return service.DownloadBase(token,svc, context);
    return service.DownloadBase(token, context);
});


taskGroupDownload.MapMethods(StreamsRoutes.Download.Base, new[] { HttpMethods.Head }, (string token, IRoutesEndpoints service, HttpContext context) =>
{
   // return service.DownloadBaseHead(token, svc, context);
    return service.DownloadBaseHead(token, context);
});






















taskGroupDev.MapPut("/CarregarDoBanco", async (IRoutesEndpoints service) =>
{
    var resultado = await service.CarregarDoBancoAsync();
    return resultado;
});




app.Run();