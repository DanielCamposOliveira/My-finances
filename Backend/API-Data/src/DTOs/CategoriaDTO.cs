using API_Data.src.Enum;

namespace API_Data.src.DTOs
{
    public class CategoriaDTO
    {
        public record CriarCategoriaDto(string Nome, Atribuicao Atribuicao);
        public record CategoriaResponseDto(int Id, string Nome, Atribuicao Atribuicao);
    }
}
