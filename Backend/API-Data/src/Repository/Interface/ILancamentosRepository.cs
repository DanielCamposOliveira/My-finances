using API_Data.src.DTOs.Lancamento;
using API_Data.src.Model;

namespace API_Data.src.Repository.Interface
{
    public interface ILancamentosRepository
    {
        public Task<bool> CategoriaExisteAsync(int categoriaId);

        public Task<List<Tag>> ObterTagsPorIdsAsync(List<int> tagIds);

        public Task<Lancamento?> AdicionarLancamentoAsync(Lancamento lancamento);

        public Task<List<Lancamento>?> ListaLancamentosAsync();

        public Task<List<LancamentoResponse>?> ListaTodosLancamentosAsync();

        public Task<List<LancamentoParcela>?> ListParcelasAbertasAtrasadasAsync(int LancamentoId, int ano, int mes);

        public Task<bool> UpdateLancamentoParcela(LancamentoParcela parcela);

        public Task<LancamentoParcela?> BuscaLancamentoParcelasync(int id);



    }
}
