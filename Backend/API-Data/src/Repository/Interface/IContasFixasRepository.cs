using API_Data.src.Model;

namespace API_Data.src.Repository.Interface
{
    public interface IContasFixasRepository
    {

        public Task<bool> CheckCategoriasPorIdsAsync(int categoriaId, string userId);

        public Task<List<Tag>?> ListaTagsPorIdsAsync(List<int> tagIds, string userId);

        public Task<ContaFixa?> CriarContaFixaAsync(ContaFixa contaFixa);

        public Task<List<ContaFixa>?> ListaContasFixasAtivasAsync(string userId);

        public Task<List<ContaFixa>> ListaContasFixasAsync(string userId);

        public Task<ContaFixaParcela?> ObterParcelaDoMesAsync(int contaFixaId, int ano, int mes);

        public Task<List<ContaFixaParcela>> ListParcelasAbertasAtrasadasAsync(int contaFixaId, int ano, int mes);

        public Task<ContaFixaParcela?> CriarParcelaFixaAsync(ContaFixaParcela parcela);

       // public Task<ContaFixaParcela?> ObterParcelaPorIdAsync(int parcelaId);

        public Task<bool> UpdateParcelaAsync(ContaFixaParcela parcela);

        public Task<ContaFixa?> ObterContaFixaPorIdAsync(int Id, string userId);

        public Task<bool> AtualizarStatusContaFixaAsync(ContaFixa conta);



        public Task<ContaFixaParcela> ObterParcelaAsync(int id);
        public Task<bool> ChecarContaFixa(int id, string userId);
    }
}
