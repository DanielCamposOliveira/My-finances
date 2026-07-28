namespace API_Data.src.DTOs
{
    public class TagDTO
    {
        public record CriarTagDto(string Nome);
        public record TagResponseDto(int Id, string Nome);
    }
}
