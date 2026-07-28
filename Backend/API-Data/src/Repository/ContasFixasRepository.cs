using API_Data.src.Data;
using API_Data.src.Model;
using Microsoft.EntityFrameworkCore;

namespace API_Data.src.Repository
{
    public class ContasFixasRepository
    {
        private readonly AppDbContext _db;
        public ContasFixasRepository(AppDbContext db)
        {
            _db = db;
        }


        public async Task<bool> CategoriaExisteAsync(int categoriaId)
        {
            return await _db.Categorias.AnyAsync(c => c.Id == categoriaId);
        }

        public async Task<List<Tag>> ObterTagsPorIdsAsync(List<int> tagIds)
        {
            return await _db.Tags
                .Where(t => tagIds.Contains(t.Id))
                .ToListAsync();
        }

        public async Task<ContaFixa> AdicionarContaFixaAsync(ContaFixa contaFixa)
        {
            _db.Set<ContaFixa>().Add(contaFixa);
            await _db.SaveChangesAsync();
            return contaFixa;
        }

        public async Task<List<ContaFixa>> ObterContasFixasAtivasAsync()
        {
            return await _db.Set<ContaFixa>()
                .AsNoTracking()
                .Include(cf => cf.Categoria)
                .Include(cf => cf.Tags)
                .Where(cf => cf.Ativo)
                .ToListAsync();
        }

        public async Task<Parcela?> ObterParcelaDoMesAsync(int contaFixaId, int ano, int mes)
        {
            return await _db.Parcelas
                .FirstOrDefaultAsync(p => p.ContaFixaId == contaFixaId
                                       && p.DataVencimento.Year == ano
                                       && p.DataVencimento.Month == mes);
        }

        public async Task<Parcela> CriarParcelaFixaAsync(Parcela parcela)
        {
            _db.Parcelas.Add(parcela);
            await _db.SaveChangesAsync();
            return parcela;
        }

        public async Task<string?> ObterNomeCategoriaAsync(int categoriaId)
        {
            var categoria = await _db.Categorias.FindAsync(categoriaId);
            return categoria?.Nome;
        }
    }
}
