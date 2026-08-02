using API_Data.src.DTOs;

namespace API_Data.src.Services.Interface
{
    public interface IHistoricoFinanceiroAnualService
    {
        public Task<List<GraficoHistoricoResponse>> ListaHistoricoAsync(int ano);

        public Task<IResult> AtualizarHistoricoMesAsync(HistoricoMesRequest request);

    }
}
