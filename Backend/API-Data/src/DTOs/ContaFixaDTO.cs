namespace API_Data.src.DTOs.ContasFixas
{
    public record ContaFixaUpdateStatus
    {
        public required int Id_ContaFixa { get; init; }
        public required bool Status { get; init; }
    };

    public record ContaFixaUpdatePrice
    {
        public required int Id_ContaFixa { get; init; }
        public required decimal ValorParcela { get; init; }
    };

    public record Create
    {
        public required string Descricao { get; init; }
        public required decimal ValorBase { get; init; }
        public required int DiaVencimento { get; init; }
        public required int CategoriaId { get; init; }
        public List<int>? TagIds { get; init; }
    };

    public record ContaFixaResponse
    {
        public required int Id { get; init; }
        public required string Descricao { get; init; }
        public required decimal ValorBase { get; init; }
        public required int DiaVencimento { get; init; }
        public required bool Ativo { get; init; }
        public required int CategoriaId { get; init; }
        public List<int>? TagIds { get; init; }
    };


}
