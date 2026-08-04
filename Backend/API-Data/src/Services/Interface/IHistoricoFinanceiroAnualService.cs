using API_Data.src.DTOs;

namespace API_Data.src.Services.Interface
{
    public interface IHistoricoFinanceiroAnualService
    {
        public Task<IResult> ListaHistoricoAsync(int ano);

        public Task<IResult> UpdateHistoricoMesAsync(HistoricoMesRequest request);

        public Task<IResult> GerarHistoricoMesAsync();

    }
}
