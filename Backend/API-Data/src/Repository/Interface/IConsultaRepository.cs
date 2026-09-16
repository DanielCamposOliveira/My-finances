namespace API_Data.src.Repository.Interface
{
    public interface IConsultaRepository
    {

        public Task<Decimal> TotalContasPendentes(int ano, int mes, string userId);

        public Task<Decimal> TotalReceber(int ano, int mes, string userId);

        public Task<Decimal> TotalReceitas(int ano, int mes, string userId);

        public Task<Decimal> TotalQuitadasDoMes(int ano, int mes, string userId);

        public Task<Decimal> TotalDespesas(int ano, int mes, string userId);

        public Task<Decimal> TotalContasMesFull(int ano, int mes, string userId);
    }
}
