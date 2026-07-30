using API_Data.src.Data;
using API_Data.src.Enum;
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


        // ## verificar se existe uma categoria
        public async Task<bool> CheckCategoriasPorIdsAsync(int categoriaId)
        {
            try
            {
                return await _db.Categorias.AnyAsync(c => c.Id == categoriaId);
            }
            catch
            {
            return false;
            }          
        }


        //## Obtem a lista de Tag
        public async Task<List<Tag>> ListaTagsPorIdsAsync(List<int> tagIds)
        {
            try
            {
                return await _db.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync();
            }
            catch
            {
            return new List<Tag>();
            }
        }

        // Criar a Conta Fixa
        public async Task<ContaFixa> CriarContaFixaAsync(ContaFixa contaFixa)
        {
            try
            {
                _db.Set<ContaFixa>().Add(contaFixa);
                await _db.SaveChangesAsync();
                return contaFixa;
            }
            catch
            {
                return null;
            }
        }





        // ## Lista todas as Contas Fixa Ativas
        public async Task<List<ContaFixa>> ListaContasFixasAtivasAsync()
        {
            try
            {
                return await _db.Set<ContaFixa>()
                .AsNoTracking()
                .Include(cf => cf.Categoria)
                .Include(cf => cf.Tags)
                .Where(cf => cf.Ativo)
                .ToListAsync();
            }
            catch
            {
                return new List<ContaFixa>();
            }
        }

        // ## Lista todas as Contas Fixa
        public async Task<List<ContaFixa>> ListaContasFixasAsync()
        {
            try
            {
                return await _db.Set<ContaFixa>()
                .AsNoTracking()
                .Include(cf => cf.Categoria)
                .Include(cf => cf.Tags)
                .ToListAsync();
            }
            catch
            {
                return new List<ContaFixa>();
            }
        }


        // ## Lista todas as parcelas do mes
        public async Task<Parcela?> ObterParcelaDoMesAsync(int contaFixaId, int ano, int mes)
        {
            try
            {
                return await _db.Parcelas
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
        public async Task<List<Parcela>> ListParcelasAbertasAtrasadasAsync(int contaFixaId, int ano, int mes)
        {
            try
            {
                // 1. Define o primeiro dia do mês solicitado (ex: 01/07/2026)
                var inicioMes = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Utc);

                // 2. Define o primeiro dia do próximo mês (ex: 01/08/2026)
                var fimMes = inicioMes.AddMonths(1);

                return await _db.Parcelas
                    .AsNoTracking()
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
                return new List<Parcela>();
            }
        }


        // ## Cria a parcela da Contas Fixa
        public async Task<Parcela> CriarParcelaFixaAsync(Parcela parcela)
        {
            try
            {
                _db.Parcelas.Add(parcela);
                await _db.SaveChangesAsync();
                return parcela;
            }
            catch
            {
                return null;
            }
        }



        // ## Busca a parcela
        public async Task<Parcela?> ObterParcelaPorIdAsync(int parcelaId)
        {
            return await _db.Parcelas.FindAsync(parcelaId);
        }

        // ## Atualiza o status da parcela
        public async Task<bool> AtualizarStatusParcelaAsync(Parcela parcela)
        {
            try
            {
                _db.Parcelas.Update(parcela);
                await _db.SaveChangesAsync();
                return true;
                
            }
            catch
            {
                return false;
            }

        }



        // ## Busca a ContaFixa
        public async Task<ContaFixa?> ObterContaFixaPorIdAsync(int Id)
        {
            return await _db.ContaFixa.FindAsync(Id);
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
