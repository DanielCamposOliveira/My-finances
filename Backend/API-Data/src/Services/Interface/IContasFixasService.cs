using API_Data.src.DTOs.ContasFixas;

namespace API_Data.src.Services.Interface
{
    public interface IContasFixasService
    {

        public Task<IResult> CriarContaFixaAsync(Create Dados, string userId);

        public Task<IResult> GerarFaturasMesAsync(string userId);

        public Task<IResult> ListaTodasContasFixa(string userId);

        public Task<IResult> ListFaturaPendenteAsync(string userId);

        public Task<IResult> UpdateStatusParcela(ParcelaUpdateStatus dto, string userId);

        public Task<IResult> UpdateValorParcela(ParcelaUpdateValor dto, string userId);

        public Task<IResult> UpdateStatusContaFixa(ContaFixaUpdateStatus dto, string userId);
    }
}
