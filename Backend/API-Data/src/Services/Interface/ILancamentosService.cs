using API_Data.src.DTOs.Lancamento;
using API_Data.src.Enum;

namespace API_Data.src.Services.Interface
{
    public interface ILancamentosService
    {
        public Task<List<LancamentoResponse>> ListarLancamentosAsync();

        public Task<List<ParcelasResponse>> ListFaturaPendenteAsync();

        public Task<IResult> CriarLancamentoAsync(Create dto);

        public Task<IResult> UptateStatusLancamentoParcela(int id, StatusParcela status);
    }
}
