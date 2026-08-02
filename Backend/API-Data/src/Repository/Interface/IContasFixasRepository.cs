using API_Data.src.Model;

namespace API_Data.src.Repository.Interface
{
    public interface IContasFixasRepository
    {

        public Task<bool> CheckCategoriasPorIdsAsync(int categoriaId);

        public Task<List<Tag>?> ListaTagsPorIdsAsync(List<int> tagIds);

        public Task<ContaFixa?> CriarContaFixaAsync(ContaFixa contaFixa);

        public Task<List<ContaFixa>?> ListaContasFixasAtivasAsync();

        public Task<List<ContaFixa>> ListaContasFixasAsync();

        public Task<ContaFixaParcela?> ObterParcelaDoMesAsync(int contaFixaId, int ano, int mes);

        public Task<List<ContaFixaParcela>> ListParcelasAbertasAtrasadasAsync(int contaFixaId, int ano, int mes);

        public Task<ContaFixaParcela?> CriarParcelaFixaAsync(ContaFixaParcela parcela);

        public Task<ContaFixaParcela?> ObterParcelaPorIdAsync(int parcelaId);

        public Task<bool> AtualizarStatusParcelaAsync(ContaFixaParcela parcela);

        public Task<ContaFixa?> ObterContaFixaPorIdAsync(int Id);

        public Task<bool> AtualizarStatusContaFixaAsync(ContaFixa conta);
    }
}
