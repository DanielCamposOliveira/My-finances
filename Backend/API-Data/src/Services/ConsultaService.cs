using API_Data.src.Repository.Interface;
using API_Data.src.Services.Interface;

namespace API_Data.src.Services
{
    public class ConsultaService : IConsultaService
    {
        private readonly IConsultaRepository _repository;
        public ConsultaService(IConsultaRepository repository)
        {
            _repository = repository;
        }

        public async Task<Decimal> ValorLancamento()
        {
            int ano = DateTime.Today.Year;
            int mes = DateTime.Today.Month;

            var retorno = await _repository.ValorLancamento(ano, mes);
            return retorno;
        }

        public async Task<Decimal> ValorContaFixa()
        {
            int ano = DateTime.Today.Year;
            int mes = DateTime.Today.Month;

            var retorno = await _repository.ValorContaFixa(ano, mes);
            return retorno;
        }

        public async Task<Decimal> TotalContasApgar()
        {
            int ano = DateTime.Today.Year;
            int mes = DateTime.Today.Month;

            var retorno = await _repository.TotalContasApgar(ano, mes);
            return retorno;
        }

        public async Task<Decimal> TotalContasReceber()
        {
            int ano = DateTime.Today.Year;
            int mes = DateTime.Today.Month;

            var retorno = await _repository.TotalContasReceber(ano, mes);
            return retorno;
        }

        //## 
        public async Task<Decimal> TotalSaldo()
        {
            int ano = DateTime.Today.Year;
            int mes = DateTime.Today.Month;

            var retorno = await _repository.TotalContasSaldo(ano, mes);
            return retorno;
        }

        // Dividas que pague esse mes
        public async Task<Decimal> TotalPagasDoMes()
        {
            int ano = DateTime.Today.Year;
            int mes = DateTime.Today.Month;

            var retorno = await _repository.TotalPagasDoMes(ano, mes);
            return retorno;
        }

        // RASCUNHO: Isso significa que vai buscar todas as dividas que foram criadas no mês, independente de estarem pagas ou não.
        public async Task<Decimal> TotalDividasMes()
        {
            int ano = DateTime.Today.Year;
            int mes = DateTime.Today.Month;
            var retorno = await _repository.TotalDividasMes(ano, mes);
            return retorno;
        }
    }
}

