using API_Data.src.DTOs.Result;
using API_Data.src.Services.Interface;
using System.Net.Http.Headers;
using System.Text.Json;

namespace API_Data.src.Services
{
    public class ArquivosService : IArquivosService
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger<ArquivosService> _logger;
        private readonly string ExternalApi_Key;
        private readonly string ExternalApi_Protocolo;
        private readonly string ExternalApi_Host;
        private readonly string ExternalApi_Port;


        public ArquivosService(HttpClient httpClient, IConfiguration configuration, ILogger<ArquivosService> logger)
        {
            _httpClient = httpClient;
        

            ExternalApi_Key = configuration.GetValue<string>("ExternalApi:BaseKey");
            ExternalApi_Protocolo = configuration.GetValue<string>("ExternalApi:Protocolo");
            ExternalApi_Host = configuration.GetValue<string>("ExternalApi:BaseUrl");
            ExternalApi_Port = configuration.GetValue<string>("ExternalApi:Port");

            _logger = logger;     
        }

        public async Task<IResult> Download_Old(string token)
        {
            try
            {

                var baseUrl = $"{ExternalApi_Protocolo}://{ExternalApi_Host}:{ExternalApi_Port}";
                var requestUrl = $"{baseUrl}/api/v1/output/{token}";


                // Configura a requisição HTTP GET e injeta o token JWT no Header Bearer
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

                // Adiciona o accept igual ao curl (-H 'accept: */*')
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

                // Adiciona a chave de API
                if (!string.IsNullOrEmpty(ExternalApi_Key))
                {
                    request.Headers.Add("X-Api-Key", ExternalApi_Key);
                }

                // Executa a requisição para a API externa
                var response = await _httpClient.SendAsync(request);

                // Se a resposta não for sucesso (aceitando códigos 200 até 299, onde o 201 se encaixa)
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return Results.BadRequest(new { mensagem = "Erro na API externa.", status = (int)response.StatusCode, detalhe = errorContent });
                }

                // Lê o conteúdo JSON retornado pela API (que possui a estrutura { "url": "..." })
                var jsonContent = await response.Content.ReadAsStringAsync();


                try
                {
                    using var doc = JsonDocument.Parse(jsonContent);
                    if (doc.RootElement.ValueKind == JsonValueKind.String)
                    {
                        return Results.Ok(new { url = doc.RootElement.GetString() });
                    }
                    else if (doc.RootElement.TryGetProperty("url", out var urlElement))
                    {
                        return Results.Ok(new { url = urlElement.GetString() });
                    }
                }
                catch (JsonException)
                {
                    // Se não for um JSON válido, retorna a string pura tratada
                    if (!string.IsNullOrEmpty(jsonContent))
                    {
                        return Results.Ok(new { url = jsonContent.Trim('"') });
                    }
                }

                return Results.Ok(new { url = jsonContent });

            }
            catch (Exception ex)
            {
                return Results.Problem($"Erro interno ao processar o download: {ex.Message}");
            }
        }

        public async Task<OperationResult> Download(string token)
        {
            try
            {

                var baseUrl = $"{ExternalApi_Protocolo}://{ExternalApi_Host}:{ExternalApi_Port}";
                var requestUrl = $"{baseUrl}/api/v1/output/{token}";


                // Configura a requisição HTTP GET e injeta o token JWT no Header Bearer
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

                // Adiciona o accept igual ao curl (-H 'accept: */*')
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

                // Adiciona a chave de API
                if (!string.IsNullOrEmpty(ExternalApi_Key))
                {
                    request.Headers.Add("X-Api-Key", ExternalApi_Key);
                }

                // Executa a requisição para a API externa
                var response = await _httpClient.SendAsync(request);

                // Se a resposta não for sucesso (aceitando códigos 200 até 299, onde o 201 se encaixa)
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning($"Erro na API externa. Status: {(int)response.StatusCode}, detalhe: {errorContent}");

                    return new OperationResult
                    {
                        Success = false,
                        Message = $"Erro na API externa. Status: {(int)response.StatusCode}, detalhe: {errorContent}"
                    };                    
                }

                // Lê o conteúdo JSON retornado pela API (que possui a estrutura { "url": "..." })
                var jsonContent = await response.Content.ReadAsStringAsync();


                try
                {
                    using var doc = JsonDocument.Parse(jsonContent);
                    if (doc.RootElement.ValueKind == JsonValueKind.String)
                    {      
                        return new OperationResult
                        {
                            Success = true,
                            Message = $"{doc.RootElement.GetString()}"
                        };

                    }
                    else if (doc.RootElement.TryGetProperty("url", out var urlElement))
                    {
                        return new OperationResult
                        {
                            Success = true,
                            Message = $"{urlElement.GetString()}"
                        };
                    }
                }
                catch (JsonException)
                {
                    // Se não for um JSON válido, retorna a string pura tratada
                    if (!string.IsNullOrEmpty(jsonContent))
                    {
                        return new OperationResult
                        {
                            Success = true,
                            Message = $"{jsonContent.Trim('"')}"
                        };                      
                    }
                }

                return new OperationResult
                {
                    Success = true,
                    Message = $"{jsonContent}"
                };            

            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Erro interno ao processar o download:  {ex.Message}");

                return new OperationResult
                {
                    Success = false,
                    Message = $"Erro interno ao processar o download:  {ex.Message}"
                };         
            }
        }

        public async Task<IResult> Upload()
        {        
            try
            {
                             
                var baseUrl = $"{ExternalApi_Protocolo}://{ExternalApi_Host}:{ExternalApi_Port}";
                var requestUrl = $"{baseUrl}/api/v1/inbound/";


                // Configura a requisição HTTP GET e injeta o token JWT no Header Bearer
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

                // Adiciona o accept igual ao curl (-H 'accept: */*')
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

                // Adiciona a chave de API
                if (!string.IsNullOrEmpty(ExternalApi_Key))
                {
                    request.Headers.Add("X-Api-Key", ExternalApi_Key);
                }

                // Executa a requisição para a API externa
                var response = await _httpClient.SendAsync(request);

                // Se a resposta não for sucesso (aceitando códigos 200 até 299, onde o 201 se encaixa)
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    
                    _logger.LogWarning($"Erro na API externa: {errorContent} - StatusCode: {(int)response.StatusCode}");

                    return Results.BadRequest(new { mensagem = "Erro na API externa.", status = (int)response.StatusCode, detalhe = errorContent });
                }

                // Lê o conteúdo JSON retornado pela API (que possui a estrutura { "url": "..." })
                var jsonContent = await response.Content.ReadAsStringAsync();


                try
                {
                    using var doc = JsonDocument.Parse(jsonContent);
                    if (doc.RootElement.ValueKind == JsonValueKind.String)
                    {
                        return Results.Ok(new { url = doc.RootElement.GetString() });
                    }
                    else if (doc.RootElement.TryGetProperty("url", out var urlElement))
                    {
                        return Results.Ok(new { url = urlElement.GetString() });
                    }
                }
                catch (JsonException)
                {
                    // Se não for um JSON válido, retorna a string pura tratada
                    if (!string.IsNullOrEmpty(jsonContent))
                    {
                        return Results.Ok(new { url = jsonContent.Trim('"') });
                    }
                }

                return Results.Ok(new { url = jsonContent });

            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Erro na API externa: {ex.Message}");
                return Results.Problem($"Erro interno ao processar o Upload: {ex.Message}");
            }
        }
    }
}
