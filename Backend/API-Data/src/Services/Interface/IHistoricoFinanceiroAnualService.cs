using API_Data.src.DTOs;

namespace API_Data.src.Services.Interface
{
    public interface IHistoricoFinanceiroAnualService
    {
        public Task<IResult> ListaHistoricoAsync(int ano, string userId);

        public Task<IResult> UpdateHistoricoMesAsync(HistoricoMesRequest request, string userId);

        public Task<IResult> GerarHistoricoMesAsync(string userId);

    }
}
