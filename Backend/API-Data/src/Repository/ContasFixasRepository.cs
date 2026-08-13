using API_Data.src.Data;
using API_Data.src.Enum;
using API_Data.src.Model;
using API_Data.src.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace API_Data.src.Repository
{
    public class ContasFixasRepository : IContasFixasRepository
    {
        private readonly AppDbContext _db;
        public ContasFixasRepository(AppDbContext db)
        {
            _db = db;
        }


        // ## verificar se existe uma categoria
        public async Task<bool> CheckCategoriasPorIdsAsync(int categoriaId, string userId)
        {
            try
            {
                var categoria = await _db.Categorias.AsNoTracking()
                                .FirstOrDefaultAsync(c => c.Id == categoriaId && c.UserId == userId);
                if(categoria == null)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        //## Obtem a lista de Tag
        public async Task<List<Tag>?> ListaTagsPorIdsAsync(List<int> tagIds, string userId)
        {
            try
            {
                return await _db.Tags.Where(t => tagIds.Contains(t.Id) && t.UserId == userId).ToListAsync();
            }
            catch
            {
                return new List<Tag>();
            }
        }

        // Criar a Conta Fixa
        public async Task<ContaFixa?> CriarContaFixaAsync(ContaFixa contaFixa)
        {
            try
            {
                _db.Set<ContaFixa>().AddAsync(contaFixa);
                await _db.SaveChangesAsync();
                return contaFixa;
            }
            catch
            {
                return null;
            }
        }

        // ## Lista todas as Contas ativas
        public async Task<List<ContaFixa>?> ListaContasFixasAtivasAsync(string userId)
        {
            try
            {
                return await _db.Set<ContaFixa>()
                    .AsNoTracking()
                    .Include(cf => cf.Categoria)
                    .Include(cf => cf.Tags)
                    .Where(cf => cf.Ativo && cf.UserId == userId)
                    .ToListAsync();
            }
            catch
            {
                return null;
            }
        }

        // ## Lista todas as Contas Fixa
        public async Task<List<ContaFixa>> ListaContasFixasAsync(string userId)
        {
            try
            {
                return await _db.Set<ContaFixa>()
                .AsNoTracking()
                .Include(cf => cf.Categoria)
                .Include(cf => cf.Tags)
                .Where(cf => cf.UserId == userId)
                .ToListAsync();
            }
            catch
            {
                return null;
            }
        }

        // ## Lista todas as parcelas do mes
        public async Task<ContaFixaParcela?> ObterParcelaDoMesAsync(int contaFixaId, int ano, int mes)
        {
            try
            {
                return await _db.ContaFixaParcelas
                    .AsNoTracking()
                                .FirstOrDefaultAsync(p => p.ContaFixaId == contaFixaId
                               && p.DataVencimento.Year == ano
                               && p.DataVencimento.Month == mes);
            }
            catch
            {
                return null;
            }
        }

        // ## Buscar todas as parcelas do Mes atual que NÃO esteja como PAGO por ID da CONTA
        public async Task<List<ContaFixaParcela>> ListParcelasAbertasAtrasadasAsync(int contaFixaId, int ano, int mes)
        {
            try
            {
                // 1. Define o primeiro dia do mês solicitado (ex: 01/07/2026)
                var inicioMes = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Utc);

                // 2. Define o primeiro dia do próximo mês (ex: 01/08/2026)
                var fimMes = inicioMes.AddMonths(1);

                return await _db.ContaFixaParcelas
                    .AsNoTracking()
                     .Include(p => p.ContaFixa)
                     .ThenInclude(p => p.Categoria)
                                                     
                    .Where(p => p.ContaFixaId == contaFixaId && p.Status != StatusParcela.Pago &&
                        (
                            // Condição 1: Do mês solicitado e não pago
                            (p.DataVencimento >= inicioMes && p.DataVencimento < fimMes)

                            ||

                            // Condição 2: Vencidas ANTES do mês atual que continuam abertas/atrasadas
                            (p.DataVencimento < inicioMes)
                        )
                    )
                    .OrderBy(p => p.DataVencimento)
                    .ToListAsync();
            }
            catch
            {
                return new List<ContaFixaParcela>();
            }
        }

        // ## Cria a parcela da Contas Fixa
        public async Task<ContaFixaParcela?> CriarParcelaFixaAsync(ContaFixaParcela parcela)
        {
            try
            {
                _db.ContaFixaParcelas.AddAsync(parcela);
                await _db.SaveChangesAsync();
                return parcela;
            }
            catch
            {
                return null;
            }
        }

        // ## Atualiza o status da parcela
        public async Task<bool> UpdateParcelaAsync(ContaFixaParcela parcela)
        {
            try
            {
                _db.ContaFixaParcelas.Update(parcela);
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Obtem o objeto parcela para edição
        public async Task<ContaFixaParcela> ObterParcelaAsync(int id)
        {
            try
            {                
                return await _db.ContaFixaParcelas.FindAsync(id);
            }
            catch
            {
                return null;
            }
        }

        // verifica se a contaFixa é do Usuario
        public async Task<bool> ChecarContaFixa(int id, string userId)
        {
            try
            {
                var retorno = await _db.ContaFixa.AsNoTracking().FirstAsync(c => c.Id == id && c.UserId == userId);
                if(retorno == null)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }            
        }

        // ## Busca a ContaFixa
        public async Task<ContaFixa?> ObterContaFixaPorIdAsync(int Id, string userId)
        {
            try
            {
                return await _db.ContaFixa.FirstOrDefaultAsync(c => c.Id == Id && c.UserId == userId);
            }
            catch
            {
                return null;
            }
        }

        // ## Atualiza o status da ContaFixa
        public async Task<bool> AtualizarStatusContaFixaAsync(ContaFixa conta)
        {
            try
            {
                _db.ContaFixa.Update(conta);
                await _db.SaveChangesAsync();
                return true;

            }
            catch
            {
                return false;
            }
        }


    }
}