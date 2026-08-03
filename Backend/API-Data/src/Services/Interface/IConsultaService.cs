namespace API_Data.src.Services.Interface
{
    public interface IConsultaService
    {
        public Task<Decimal> TotalContasPendentes();
        public Task<Decimal> TotalReceber();
        public Task<Decimal> TotalSaldo();

        public Task<Decimal> TotalQuitadasDoMes();
        public Task<Decimal> TotalDividasMes(); 
    }
}
