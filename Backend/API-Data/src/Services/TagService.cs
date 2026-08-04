using API_Data.src.DTOs;
using API_Data.src.Model;
using API_Data.src.Repository.Interface;
using API_Data.src.Services.Interface;

namespace API_Data.src.Services
{
    public class TagService : ITagService
    {

        private readonly ITagRepository _tagRepository;
        public TagService(ITagRepository tag)
        {
            _tagRepository = tag;
        }

        public async Task<IResult>CriarTag(CriarTagDto tag)
        {
            var Dados = new Tag
            {
                Nome = tag.Nome,
            };

            var retorno = await _tagRepository.CriarTag(Dados);

            if (retorno == null)
            {
                return Results.Problem(
                "Erro ao criar Tag",
                statusCode: StatusCodes.Status500InternalServerError);          
            }

            return Results.Created();
        }


        public async Task<List<TagResponseDto>> ListaTags()
        {
           var retorno = await _tagRepository.ListaTags();
           if(retorno == null)
           {
             var lista = new List<TagResponseDto>();
             return lista;
           }              

            return retorno;
        }

    }

}
