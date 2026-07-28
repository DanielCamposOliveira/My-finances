namespace API_Data.src.DTOs
{
    public record ContaFixaResponseDto(
        int Id,
        string Descricao,
        decimal ValorBase,
        int DiaVencimento,
        bool Ativo,
        string CategoriaNome,
        List<string> Tags
    );
}
