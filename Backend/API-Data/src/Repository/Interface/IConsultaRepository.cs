namespace API_Data.src.Repository.Interface
{
    public interface IConsultaRepository
    {

        public Task<Decimal> TotalContasPendentes(int ano, int mes);

        public Task<Decimal> TotalReceber(int ano, int mes);

        public Task<Decimal> TotalSaldo(int ano, int mes);

        public Task<Decimal> TotalQuitadasDoMes(int ano, int mes);

        public Task<Decimal> TotalDividasMes(int ano, int mes);
    }
}
