using API_Data.src.Enum;

namespace API_Data.src.DTOs
{

    public record CategoriaResponseDto
    {
        public required int Id { get; set; }
        public required string Nome { get; init; }
        public required Atribuicao Atribuicao { get; init; }
    }

    public record CriarCategoriaDto
    {
        public required string Nome { get; init; }
        public required Atribuicao Atribuicao { get; init; }
    }


}
