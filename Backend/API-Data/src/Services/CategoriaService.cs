using API_Data.src.DTOs;
using API_Data.src.Model;
using API_Data.src.Repository.Interface;
using API_Data.src.Services.Interface;

namespace API_Data.src.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;
            
        public CategoriaService(ICategoriaRepository repo)
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

            if(retorno == false)
            {
                return Results.Problem(
                "Erro ao criar Categoria",
                statusCode: StatusCodes.Status500InternalServerError);
            }

            return Results.Created();
        }

        public async Task<IResult> ListaCategoria()
        {
            var retorno = await _categoriaRepository.ListaCategoriaAsync();
           
            if(retorno == null)
            { 
                return Results.Problem(
                "Erro ao criar Categoria",
                statusCode: StatusCodes.Status500InternalServerError);
            }

            return Results.Ok(retorno);
        }
    }
}
