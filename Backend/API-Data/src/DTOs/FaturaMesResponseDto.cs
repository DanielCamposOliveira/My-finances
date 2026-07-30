using API_Data.src.Enum;

namespace API_Data.src.DTOs
{
    public record FaturaMesResponseDto(
        int ParcelaId,
        int ContaFixaId,
        string Descricao,
        decimal ValorParcela,
        DateTime DataVencimento,
        DateTime? DataPagamento,
        StatusParcela Status
    );

}
