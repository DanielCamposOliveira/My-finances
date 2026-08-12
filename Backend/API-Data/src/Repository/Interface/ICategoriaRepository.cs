using API_Data.src.DTOs;
using API_Data.src.Model;

namespace API_Data.src.Repository.Interface
{
    public interface ICategoriaRepository
    {
        public Task<bool?> CriarCategoriaAsync(Categoria categoria);

        public Task<bool?> CriarListCategoriaAsync(List<Categoria> categoria);

        public Task<List<CategoriaResponseDto>> ListaCategoriaAsync();
    }
}
