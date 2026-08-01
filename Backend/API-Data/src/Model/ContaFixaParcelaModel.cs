using API_Data.src.Enum;

namespace API_Data.src.Model
{
    public class ContaFixaParcela
    {
        public int Id { get; set; }
        public int NumeroParcela { get; set; }
        public decimal ValorParcela { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime? DataPagamento { get; set; }
        public StatusParcela Status { get; set; } = StatusParcela.Aberto;

        // Relacionamento Obrigatório com ContaFixa
        public int ContaFixaId { get; set; }
        public ContaFixa ContaFixa { get; set; } = null!;
    }
}
