using API_Data.src.Enum;

namespace API_Data.src.DTOs
{

///    public record CategoriaResponseDtos(int Id, string Nome, Atribuicao Atribuicao);

    public record CategoriaResponseDto
    {
        public required string Nome { get; init; }
        public required Atribuicao Atribuicao { get; init; }
    }

    public record CriarCategoriaDto
    {
        public required string Nome { get; init; }
        public required Atribuicao Atribuicao { get; init; }
    }


}
