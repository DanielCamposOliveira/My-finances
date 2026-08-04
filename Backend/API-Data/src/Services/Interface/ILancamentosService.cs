using API_Data.src.DTOs.Lancamento;
using API_Data.src.Enum;

namespace API_Data.src.Services.Interface
{
    public interface ILancamentosService
    {
        public Task<IResult> ListarLancamentosAsync();

        public Task<IResult> ListFaturaPendenteAsync();

        public Task<IResult> CriarLancamentoAsync(Create dto);

        public Task<IResult> UptateStatusLancamentoParcela(int id, StatusParcela status);

       




    }
}
