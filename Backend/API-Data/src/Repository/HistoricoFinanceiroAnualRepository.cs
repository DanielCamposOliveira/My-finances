using API_Data.src.Data;
using API_Data.src.DTOs;
using API_Data.src.Model;
using API_Data.src.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace API_Data.src.Repository
{
    public class HistoricoFinanceiroAnualRepository : IHistoricoFinanceiroAnualRepository
    {
        private readonly AppDbContext _db;

        public HistoricoFinanceiroAnualRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<HistoricoFinanceiroAnual>> ObterTodosHistoricosAsync(int ano)
        {
            return await _db.HistoricosFinanceiros
              .AsNoTracking()
                .Where(h => h.Ano == ano)
                .ToListAsync();
        }


        // Implementação do método AtualizarHistoricoMesAsync
        public async Task<Boolean> AtualizarHistoricoMesAsync(HistoricoMesRequest request)
        {
            try
            {
                var registro = await _db.HistoricosFinanceiros
                .FirstOrDefaultAsync(h => h.Ano == request.ano && h.Mes == request.mes);

                if (registro == null)
                {
                    registro = new HistoricoFinanceiroAnual
                    {
                        Ano = request.ano,
                        Mes = request.mes,
                        TotalSaldo = request.novoSaldo,
                        TotalDivida = request.novaDivida
                    };
                    _db.HistoricosFinanceiros.Add(registro);
                }
                else
                {
                    registro.TotalSaldo = request.novoSaldo;
                    registro.TotalDivida = request.novaDivida;
                }

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
