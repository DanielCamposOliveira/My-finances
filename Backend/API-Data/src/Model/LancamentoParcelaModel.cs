using API_Data.src.Enum;

namespace API_Data.src.Model
{
    public class LancamentoParcela
    {
        public int Id { get; set; }
        public int NumeroParcela { get; set; }
        public decimal ValorParcela { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime? DataPagamento { get; set; }
        public StatusParcela Status { get; set; } = StatusParcela.Aberto;

        // Relacionamento Obrigatório com Lancamento
        public int LancamentoId { get; set; }
        public Lancamento Lancamento { get; set; } = null!;
    }
}
