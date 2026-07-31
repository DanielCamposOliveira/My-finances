namespace API_Data.src.DTOs
{
    public record ContaFixaUpdateStatusDTO
    {
        public required int Id_ContaFixa { get; init; }
        public required bool Status { get; init; }
    };


    public record ContaFixaCreateDTO
    {
        public required string Descricao { get; init; }
        public required decimal ValorBase { get; init; }
        public required int DiaVencimento { get; init; }
        public required int CategoriaId { get; init; }
        public List<int> TagIds { get; init; }
    };




}
