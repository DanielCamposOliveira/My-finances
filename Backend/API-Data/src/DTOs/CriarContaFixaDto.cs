namespace API_Data.src.DTOs
{
    public record CriarContaFixaDto(
        string Descricao,
        decimal ValorBase,
        int DiaVencimento,
        int CategoriaId,
        List<int> TagIds
    );
}
