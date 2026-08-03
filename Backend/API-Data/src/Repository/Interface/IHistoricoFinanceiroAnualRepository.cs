using API_Data.src.DTOs;
using API_Data.src.Model;

namespace API_Data.src.Repository.Interface
{
    public interface IHistoricoFinanceiroAnualRepository
    {
        public Task<List<HistoricoFinanceiroAnual>> ObterTodosHistoricosAsync(int ano);

        public Task<Boolean> AtualizarHistoricoMesAsync(HistoricoMesRequest request);

        public Task<List<HistoricoFinanceiroAnual>> ObterHistoricosMesAsync(int mes, int ano);


    }
}
