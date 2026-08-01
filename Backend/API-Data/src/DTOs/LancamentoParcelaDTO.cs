using API_Data.src.Enum;

namespace API_Data.src.DTOs.Lancamento
{
    public record ParcelasResponse
    {
        public required int Id { get; init; }
        public required int NumeroParcela { get; init; }
        public required decimal ValorParcela { get; init; }
        public required DateTime DataVencimento { get; init; }
        public required StatusParcela Status { get; init; }

        public required string Lancamento_Descricao { get; init; }
        public required int Lancamento_Id { get; init; }
    }

    public record ParcelaUpdateStatus
    {
        public required int ParcelaId { get; init; }
        public required StatusParcela Status { get; init; }
    }


    public record ParcelaUpdateValor
    {
        public required int ParcelaId { get; init; }
        public required decimal ValorParcela { get; init; }
    }
}
