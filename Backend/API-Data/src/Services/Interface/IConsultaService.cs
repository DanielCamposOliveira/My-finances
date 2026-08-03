namespace API_Data.src.Services.Interface
{
    public interface IConsultaService
    {
        public Task<Decimal> ValorLancamento();
        public Task<Decimal> ValorContaFixa();
        public Task<Decimal> TotalContasApgar();
        public Task<Decimal> TotalContasReceber();
        public Task<Decimal> TotalSaldo();

        public Task<Decimal> TotalPagasDoMes();
        public Task<Decimal> TotalDividasMes(); 
    }
}
