using Cloud_API.src.DTO;
using Cloud_API.src.Repository.Dictionary;

namespace Cloud_API.src.Service.Consulta
{
    public class ConsultaService : IConsultaService
    {
        private readonly DictionaryRepository _DictionaryRepository;

        public ConsultaService(DictionaryRepository dictionaryRepository)
        {
            _DictionaryRepository = dictionaryRepository;
        }
        public async Task<IResult> ObterRegistro(string token)
        {
            DocumentoDownloadDTO Dados = await _DictionaryRepository.ObterRegistro(token);

            if(Dados == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(Dados);
        }

        public async Task<IResult> ObterTodosRegistro()
        {
            var Dados = await _DictionaryRepository.ConsultarTodos();

            if (Dados == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(Dados);
        }
    }
}
