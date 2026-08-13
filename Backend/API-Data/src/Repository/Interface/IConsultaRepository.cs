namespace API_Data.src.Repository.Interface
{
    public interface IConsultaRepository
    {

        public Task<Decimal> TotalContasPendentes(int ano, int mes, string userId);

        public Task<Decimal> TotalReceber(int ano, int mes, string userId);

        public Task<Decimal> TotalSaldo(int ano, int mes, string userId);

        public Task<Decimal> TotalQuitadasDoMes(int ano, int mes, string userId);

        public Task<Decimal> TotalDividasMes(int ano, int mes, string userId);
    }
}
