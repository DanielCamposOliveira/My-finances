namespace API_Data.src.Model
{
    public class HistoricoFinanceiroAnual
    {
        public int Id { get; set; }
        public int Ano { get; set; } // data referente
        public int Mes { get; set; } // data referente
        public decimal Receitas { get; set; } // varlor recebido no mes
        public decimal Despesas { get; set; }  // varlor despesa do mes

        public decimal DespesasPagas { get; set; } // valor pago das despesas do mes e das dividas atrazadas
        public decimal DividasAnteriores { get; set; } // valor das dividas dos meses anterior em aberto

        // Relacionamento com User
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

    }
}
 