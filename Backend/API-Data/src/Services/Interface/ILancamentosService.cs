using API_Data.src.DTOs.Lancamento;
using API_Data.src.Enum;

namespace API_Data.src.Services.Interface
{
    public interface ILancamentosService
    {
        public Task<IResult> ListarLancamentosAsync(string userId);

        public Task<IResult> ListFaturaPendenteAsync(string userId);

        public Task<IResult> CriarLancamentoAsync(Create dto, string userId);

        public Task<IResult> UptateStatusLancamentoParcela(ParcelaUpdateStatus dto, string userId);

       




    }
}
