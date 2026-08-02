using API_Data.src.DTOs;

namespace API_Data.src.Services.Interface
{
    public interface ICategoriaService
    {
        public Task<IResult> CriarCategoria(CriarCategoriaDto dto);

        public Task<List<CategoriaResponseDto>> ListaCategoria();
    }
}
