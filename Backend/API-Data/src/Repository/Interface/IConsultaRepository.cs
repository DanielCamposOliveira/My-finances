namespace API_Data.src.Repository.Interface
{
    public interface IConsultaRepository
    {
        public Task<Decimal> ValorLancamento(int ano, int mes);

        public Task<Decimal> ValorContaFixa(int ano, int mes);

        public Task<Decimal> TotalContasApgar(int ano, int mes);

        public Task<Decimal> TotalContasReceber(int ano, int mes);

        public Task<Decimal> TotalContasSaldo(int ano, int mes);

        public Task<Decimal> TotalPagasDoMes(int ano, int mes);

        public Task<Decimal> TotalDividasMes(int ano, int mes);
    }
}
