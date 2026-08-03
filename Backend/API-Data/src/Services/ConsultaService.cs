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




        #region Rotas validadas

        // RASCUNHO: Soma tudo que estejam com vencimento até o mês/ano informado e seja com status "Aberto" e atribuição "Ganho"
        public async Task<Decimal> TotalReceber()
        {
            int ano = DateTime.Today.Year;
            int mes = DateTime.Today.Month;

            var retorno = await _repository.TotalReceber(ano, mes);
            return retorno;
        }



        // RASCUNHO: Isso significa que vai buscar todas as dividas que foram pagas no mês 
        public async Task<Decimal> TotalSaldo()
        {
            int ano = DateTime.Today.Year;
            int mes = DateTime.Today.Month;

            var retorno = await _repository.TotalSaldo(ano, mes);
            return retorno;
        }


        // RASCUNHO: Isso significa que vai buscar todas as dividas que foram pagas no mês
        public async Task<Decimal> TotalQuitadasDoMes()
        {
            int ano = DateTime.Today.Year;
            int mes = DateTime.Today.Month;

            var retorno = await _repository.TotalQuitadasDoMes(ano, mes);
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


        // RASCUNHO: Retorna o valor total das todas dividas que foram criadas no mês que esta em aberto e as contas Atrasado dos meses anteriores
        public async Task<Decimal> TotalContasPendentes()
        {
            int ano = DateTime.Today.Year;
            int mes = DateTime.Today.Month;

            var retorno = await _repository.TotalContasPendentes(ano, mes);
            return retorno;
        }

        #endregion
    }
}

