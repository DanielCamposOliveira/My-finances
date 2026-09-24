using API_Data.src.DTOs.Lancamento;
using API_Data.src.Model;

namespace API_Data.src.Repository.Interface
{
    public interface ILancamentosRepository
    {
        public Task<bool> CategoriaExisteAsync(int categoriaId, string userId);

        public Task<List<Tag>> ObterTagsPorIdsAsync(List<int> tagIds, string userId);

        public Task<Lancamento?> AdicionarLancamentoAsync(Lancamento lancamento);

        public Task<List<Lancamento>?> ListaLancamentosAsync(string userId);

        public Task<List<LancamentoResponseList>?> ListaTodosLancamentosAsync(string userId);
               

        public Task<List<LancamentoParcela>?> ListParcelasAbertasAtrasadasAsync(int LancamentoId, int ano, int mes);

        public Task<bool> UpdateLancamentoParcela(LancamentoParcela parcela);

        public Task<LancamentoParcela?> BuscaLancamentoParcelasync(int id);

        public Task<Lancamento?> BuscaLancamentoasync(int LancamentoId);

        public Task<List<ParcelasResponse>?> ListaTodasParcelasAsync(string userId);


        public Task<LancamentoParcela> ObterParcelaAsync(int id);
        public Task<bool> ChecarLancamentoParcela(int id, string userId);
        public Task<bool> UpdateParcelaAsync(LancamentoParcela parcela);


       // public Task<List<LancamentoResponseList>?> ListaTodosLancamentos(string userId);

    }
}
