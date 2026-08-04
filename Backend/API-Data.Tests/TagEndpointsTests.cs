using System.Net;
using System.Net.Http.Json;
using API_Data.src.DTOs;
using Xunit;

namespace API_Data.Tests
{
    public class TagEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        // HttpClient para enviar requisições HTTP para a API
        private readonly HttpClient _client;

        public TagEndpointsTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_CriarTag_DeveRetornarStatus201Created()
        {
            // Arrange
            // Cria uma tag com um nome único para evitar conflitos
            var novaTag = new CriarTagDto
            {
                Nome = $"Tag_Teste_{Guid.NewGuid():N}"
            };

            // Act
            // Envia a requisição POST para criar a tag
            var response = await _client.PostAsJsonAsync("/api/v1/tags", novaTag);

            // Assert
            // Valida se o status code retornado é 201 Created
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Get_ListaTags_DeveRetornarStatus200OK()
        {
            // Act
            // Envia a requisição GET para listar as tags
            var response = await _client.GetAsync("/api/v1/tags");

            // Assert
            // Valida se o status code retornado é 200 OK
            response.EnsureSuccessStatusCode();

            // Valida se a lista de tags não está vazia
            var tags = await response.Content.ReadFromJsonAsync<List<TagResponseDto>>();
            Assert.NotNull(tags);// Valida se a lista de tags não é nula
            Assert.NotEmpty(tags); // Valida com os registros reais salvos no seu banco
        }
    }
}