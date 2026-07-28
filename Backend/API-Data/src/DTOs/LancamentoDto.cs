using API_Data.src.Enum;

namespace API_Data.src.DTOs
{
    public class LancamentoDto
    {
        public record CriarLancamentoDto(
        string Descricao,
        decimal ValorTotal,
        TipoLancamento Tipo,
        int QtdParcelas,
        DateTime DataPrimeiroVencimento,
        int CategoriaId,
        List<int> TagIds
    );

        public record ParcelaResponseDto(
            int Id,
            int NumeroParcela,
            decimal ValorParcela,
            DateTime DataVencimento,
            DateTime? DataPagamento,
            StatusParcela Status
        );

        public record LancamentoResponseDto(
            int Id,
            string Descricao,
            decimal ValorTotal,
            TipoLancamento Tipo,
            int QtdParcelas,
            string CategoriaNome,
            List<string> Tags,
            List<ParcelaResponseDto> Parcelas
        );
    }
}
