namespace API_Data.src.DTOs
{
    public class TagDTO
    {
        public record CriarTagDto
        {
            public required string Nome { get; init; }
        }
        
        public record TagResponseDto
        {
            public required int Id { get; init; }
            public required string Nome { get; init; }
        
        }
        
        
       
    }
}
