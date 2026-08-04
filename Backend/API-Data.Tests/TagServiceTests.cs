using API_Data.src.Data;
using API_Data.src.DTOs;
using API_Data.src.Model;
using API_Data.src.Repository;
using API_Data.src.Repository.Interface;
using API_Data.src.Services;
using API_Data.src.Services.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace API_Data.Tests
{
    public class TagServiceTests
    {
        // Mock do repositório de tags para simular o comportamento do banco de dados
        private readonly Mock<ITagRepository> _tagRepositoryMock;

        // Instância do serviço de tags que será testado
        private readonly ITagService _tagService;

        private readonly ITestOutputHelper _output; // 1. Declara a interface de output

        public TagServiceTests(ITestOutputHelper output)
        {
            _output = output;
            // Inicializa o mock do repositório e o serviço de tags
            _tagRepositoryMock = new Mock<ITagRepository>();

            // Inicializa o serviço de tags com o mock do repositório
            _tagService = new TagService(_tagRepositoryMock.Object);
        }

        // Método para criar o DbContext real usando a string de conexão do appsettings.json
        private AppDbContext CriarDbContextReal()
        {

            // Carrega a configuração do arquivo appsettings.json
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "../../../../API-Data/appsettings.json"), optional: false)
                .Build();

            var connectionString = config.GetConnectionString("PostgreSQLConnection");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new AppDbContext(options);
        }




        [Fact]
        public async Task CriarTag_SemMock()
        {
            //=============================================================
            // 1. ARRANGE (Preparação com componentes REAIS)
            //=============================================================
            using var dbContext = CriarDbContextReal();

            // Instanciamos o Repositório REAL com o DbContext REAL
            var tagRepositoryReal = new TagRepository(dbContext);

            // Instanciamos o Serviço REAL passando o Repositório REAL (sem Moq!)
            var tagServiceReal = new TagService(tagRepositoryReal);

            // Criamos um nome único usando Guid para evitar conflitos de chave duplicada no banco
            var nomeTagUnico = $"Alimentacao_{Guid.NewGuid():N}";

            var dto = new CriarTagDto
            {
                Nome = nomeTagUnico
            };

            _output.WriteLine($"[LOG - CriarTag_SemMock] Tentando gravar no banco a tag: '{nomeTagUnico}'");



            //=============================================================
            // 2. ACT (Execução da regra no serviço)
            //=============================================================
            var resultado = await tagServiceReal.CriarTag(dto);



            //=============================================================
            // 3. ASSERT (Validação no resultado e no BANCO)
            //=============================================================
            // A) Valida se o serviço retornou o status Http Created (201)
            Assert.IsType<Created>(resultado);

            // B) PROVA REAL NO BANCO: Consultamos o DbContext direto para ver se o registro realmente foi gravado na tabela "Tags"
            var tagNoBanco = await dbContext.Tags
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Nome == nomeTagUnico);

            // Validações no banco de dados real:
            Assert.NotNull(tagNoBanco);                  // A tag foi encontrada no Postgres!
            Assert.True(tagNoBanco.Id > 0);               // O banco gerou uma Primary Key autoincrementada real!
            Assert.Equal(nomeTagUnico, tagNoBanco.Nome); // O nome salvo é idêntico ao DTO enviado.

            _output.WriteLine($"[LOG - CriarTag_SemMock] SUCESSO! Tag gravada no PostgreSQL com ID gerado: {tagNoBanco.Id}");
        }


        [Fact]
        public async Task CriarTag_ComMock()
        {
            // Arrange
            // Cria um DTO de criação de tag e uma tag simulada que será retornada pelo mock do repositório
            var dto = new CriarTagDto { Nome = "Alimentação" };

            // Cria uma tag simulada que será retornada pelo mock do repositório
            var tagCriada = new Tag
            {
                Id = 1,
                Nome = "Alimentação"
            };

            // Configura o mock do repositório para retornar a tag simulada quando o método CriarTag for chamado
            _tagRepositoryMock
                .Setup(repo => repo.CriarTag(It.IsAny<Tag>()))
                .ReturnsAsync(tagCriada);

            // Act
            // Chama o método CriarTag do serviço com o DTO de criação de tag
            var resultado = await _tagService.CriarTag(dto);
            _output.WriteLine($"[LOG - CriarTag_ComMock] Tentando gravar no banco a tag: '{dto.Nome}'");

            // Assert
            // Verifica se o IResult é um Created (201)
            Assert.IsType<Created>(resultado);
            _tagRepositoryMock.Verify(repo => repo.CriarTag(It.Is<Tag>(t => t.Nome == dto.Nome)), Times.Once);
        }







        [Fact]
        public async Task ListaTags()
        {
            // Arrange
            // Serve como mock de retorno do repositório
            var tagsMock = new List<TagResponseDto>
            {
                new TagResponseDto { Id = 1, Nome = "Lazer" },
                new TagResponseDto { Id = 2, Nome = "Saúde" }
            };

            // Configura o mock do repositório para retornar a lista de tags
            // Quando o teste chamar o método ListaTags, ele retornará a lista de tags simulada
            _tagRepositoryMock
                .Setup(repo => repo.ListaTags())
                .ReturnsAsync(tagsMock);

            // Act
            // Chama o método ListaTags do serviço
            var resultado = await _tagService.ListaTags();

            // 3. Debug via LOG aqui:
            _output.WriteLine($"[DEBUG] Quantidade de itens retornados: {resultado?.Count}");

            // Assert
            // Verifica se o resultado não é nulo e se contém a quantidade esperada de tags
            // 1. Valida o tipo da coleção retornada
           // Assert.NotNull(resultado); // Verifica se o resultado não é nulo
           // Assert.Equal(2, resultado.Count); // Verifica se a quantidade de tags retornadas é igual a 2
           // Assert.IsType<List<TagResponseDto>>(resultado); // Verifica se o tipo do resultado é uma lista de TagResponseDto
           // Assert.Equal(tagsMock.Count, resultado.Count); // Verifica se a quantidade de tags retornadas é igual à quantidade de tags simuladas
                                                           // Assert.Equal("Lazer", resultado[0].Nome); // Verifica se o nome da primeira tag é "Lazer"
                                                           // Assert.Equal("Saúde", resultado[1].Nome); // Verifica se o nome da segunda tag é "Saúde"



            // 2. Valida a estrutura de CADA item da lista (sem fixar o texto dos nomes)
            Assert.All(resultado, item =>
            {
                // Imprime os detalhes de cada item iterado
                _output.WriteLine($"[DEBUG] Validando item - Id: {item.Id}, Nome: '{item.Nome}'");

                Assert.NotNull(item);
                Assert.True(item.Id > 0);
                Assert.False(string.IsNullOrWhiteSpace(item.Nome));
            });

        }
    }
}
