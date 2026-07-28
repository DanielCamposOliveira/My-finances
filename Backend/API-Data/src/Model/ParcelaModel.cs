using API_Data.src.Enum;

namespace API_Data.src.Model
{
    public class Parcela
    {
        public int Id { get; set; }
        public int NumeroParcela { get; set; }
        public decimal ValorParcela { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime? DataPagamento { get; set; }
        public StatusParcela Status { get; set; } = StatusParcela.Aberto;


        // Relacionamento com Lançamentos (Opcional se for oriundo de Conta Fixa)
        public int? LancamentoId { get; set; }
        public Lancamento? Lancamento { get; set; }


        // Relacionamento com Contas Fixas (Opcional se for de Lançamento)
        public int? ContaFixaId { get; set; }
        public ContaFixa? ContaFixa { get; set; }
    }
}
