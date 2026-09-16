namespace API_Data.src.Services.Interface
{
    public interface IConsultaService
    {
        public Task<Decimal> TotalContasPendentes(string userId);
        public Task<Decimal> TotalReceber(string userId);
        public Task<Decimal> TotalReceitas(string userId);

        public Task<Decimal> TotalQuitadasDoMes(string userId);
        public Task<Decimal> TotalDespesas(string userId);

        public Task<Decimal> TotalContasMesFull(string userId);
    }
}
