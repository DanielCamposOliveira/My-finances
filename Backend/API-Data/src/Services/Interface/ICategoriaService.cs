using API_Data.src.DTOs;

namespace API_Data.src.Services.Interface
{
    public interface ICategoriaService
    {
        public Task<IResult> CriarCategoria(CriarCategoriaDto dto, string userId);

        public Task<IResult> ListaCategoria(string userId);
    }
}
