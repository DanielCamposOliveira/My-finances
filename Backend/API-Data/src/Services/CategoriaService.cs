using API_Data.src.DTOs;
using API_Data.src.Model;
using API_Data.src.Repository;

namespace API_Data.src.Services
{
    public class CategoriaService
    {
        private readonly CategoriaRepository _categoriaRepository;
            
        public CategoriaService(CategoriaRepository repo)
        {
            _categoriaRepository = repo;
        }

        public async Task<IResult> CriarCategoria(CriarCategoriaDto dto)
        {
            var Dados = new Categoria
            {
                Nome = dto.Nome,
                Atribuicao = dto.Atribuicao,
            };

            var retorno = await _categoriaRepository.CriarCategoriaAsync(Dados);

            if(retorno == null)
            {
                return Results.Problem(
                "Erro ao criar Categoria",
                statusCode: StatusCodes.Status500InternalServerError);
            }

            return Results.Created();
        }

        public async Task<List<CategoriaResponseDto>> ListaCategoria()
        {
            var retorno = await _categoriaRepository.ListaCategoriaAsync();
           
            if(retorno == null)
            {
                var lista = new List<CategoriaResponseDto>();
                return lista;
            }

            return retorno;
        }
    }
}
