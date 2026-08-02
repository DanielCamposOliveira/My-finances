using API_Data.src.DTOs.ContasFixas;
using API_Data.src.Enum;

namespace API_Data.src.Services.Interface
{
    public interface IContasFixasService
    {

        public Task<IResult> CriarContaFixaAsync(Create Dados);

        public Task<IResult> GerarFaturasMesAsync();

        public Task<List<ContaFixaResponse>> ListaTodasContasFixa();

        public Task<List<ParcelasResponse>> ListFaturaPendenteAsync();

        public Task<IResult> UpdateStatusParcela(int Id_Parcela, StatusParcela status);

        public Task<IResult> UpdateValorParcela(int Id_Parcela, decimal ValorParcela);

        public Task<IResult> UpdateStatusContaFixa(int Id_ContaFixa, bool status);
    }
}
