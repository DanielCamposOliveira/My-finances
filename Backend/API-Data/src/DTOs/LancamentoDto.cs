using API_Data.src.Enum;

namespace API_Data.src.DTOs.Lancamento
{
        public record Create
        {
            public required string Descricao { get; init; }
            public required decimal ValorTotal { get; init; }
            public required int QtdParcelas { get; init; }
            public required DateTime DataPrimeiroVencimento { get; init; }
            public required int CategoriaId { get; init; }
            public required List<int> TagIds { get; init; }
        }


        public record ParcelaResponse
        {
            public required int Id { get; init; }
            public required int NumeroParcela { get; init; }
            public required decimal ValorParcela { get; init; }
            public required DateTime DataVencimento { get; init; }
            public required DateTime? DataPagamento { get; init; }
            public required StatusParcela Status { get; init; }
        }


        public record LancamentoResponse
        {
            public required int Id { get; init; }
            public required string Descricao { get; init; }
            public required decimal ValorTotal { get; init; }
            public required int QtdParcelas { get; init; }
            public required string CategoriaNome { get; init; }
            public required List<string> Tags { get; init; }
            public required List<ParcelaResponse> Parcelas { get; init; }
        }
    
}
