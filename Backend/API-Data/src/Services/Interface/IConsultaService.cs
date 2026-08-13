namespace API_Data.src.Services.Interface
{
    public interface IConsultaService
    {
        public Task<Decimal> TotalContasPendentes(string userId);
        public Task<Decimal> TotalReceber(string userId);
        public Task<Decimal> TotalSaldo(string userId);

        public Task<Decimal> TotalQuitadasDoMes(string userId);
        public Task<Decimal> TotalDividasMes(string userId); 
    }
}
