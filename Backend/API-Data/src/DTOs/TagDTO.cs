namespace API_Data.src.DTOs
{
    public record CriarTagDto
    {
        public string Nome { get; init; } = string.Empty;
    }

    public record TagResponseDto
    {
        public int Id { get; init; }
        public string Nome { get; init; } = string.Empty;
    }
}