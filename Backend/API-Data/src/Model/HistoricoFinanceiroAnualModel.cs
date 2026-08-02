namespace API_Data.src.Model
{
    public class HistoricoFinanceiroAnual
    {
        public int Id { get; set; }
        public int Ano { get; set; }
        public int Mes { get; set; }
        public decimal TotalSaldo { get; set; }
        public decimal TotalDivida { get; set; }
    }
}
