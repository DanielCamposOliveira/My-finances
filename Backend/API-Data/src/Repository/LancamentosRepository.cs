using API_Data.src.Data;
using API_Data.src.DTOs.Lancamento;
using API_Data.src.Enum;
using API_Data.src.Model;
using API_Data.src.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace API_Data.src.Repository;

public class LancamentosRepository : ILancamentosRepository
{
    private readonly AppDbContext _db;

    public LancamentosRepository(AppDbContext db)
    {
        _db = db;
    }

    // ## verificar se existe uma categoria
    public async Task<bool> CategoriaExisteAsync(int categoriaId)
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
    public async Task<List<Tag>> ObterTagsPorIdsAsync(List<int> tagIds)
    {
        try
        {
            return await _db.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync();
        }
        catch
        {
            return [];
        }

    }

    // ## Adiciona um novo lançamento
    public async Task<Lancamento?> AdicionarLancamentoAsync(Lancamento lancamento)
    {
        try
        {
            _db.Lancamentos.Add(lancamento);
            await _db.SaveChangesAsync();
            return lancamento;
        }
        catch
        {
            return null;
        }
    }

    // ## Lista todas as Contas
    public async Task<List<Lancamento>?> ListaLancamentosAsync()
    {
        try
        {
            return await _db.Set<Lancamento>()
            .AsNoTracking()
            .Include(cf => cf.Categoria)
            .Include(cf => cf.Tags)
            .ToListAsync();
        }
        catch
        {
            return [];
        }
    }


    // Lista todos os lançamentos com suas categorias, tags e parcelas
    public async Task<List<LancamentoResponse>?> ListaTodosLancamentosAsync()
    {
        try
        {
            return await _db.Lancamentos
                .AsNoTracking()
                .Select(l => new LancamentoResponse
                {
                    Id = l.Id,
                    Descricao = l.Descricao,
                    ValorTotal = l.ValorTotal,
                    QtdParcelas = l.QtdParcelas,
                    CategoriaNome = l.Categoria.Nome,
                    Tags = l.Tags
                        .Select(t => t.Nome)
                        .ToList(),
                    Parcelas = l.Parcelas
                        .Select(p => new ParcelaResponse
                        {
                            Id = p.Id,
                            NumeroParcela = p.NumeroParcela,
                            ValorParcela = p.ValorParcela,
                            DataVencimento = p.DataVencimento,
                            DataPagamento = p.DataPagamento,
                            Status = p.Status
                        })
                        .ToList()
                })
                .ToListAsync();
        }
        catch
        {
            return [];
        }
    }


    // ## Buscar todas as parcelas do Mes atual que NÃO esteja como PAGO por ID da CONTA
    public async Task<List<LancamentoParcela>?> ListParcelasAbertasAtrasadasAsync(int LancamentoId, int ano, int mes)
    {
        try
        {
            // 1. Define o primeiro dia do mês solicitado (ex: 01/07/2026)
            var inicioMes = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Utc);

            // 2. Define o primeiro dia do próximo mês (ex: 01/08/2026)
            var fimMes = inicioMes.AddMonths(1);

            return await _db.LancamentoParcelas
                .AsNoTracking()
                .Include(p => p.Lancamento)
                .Where(p => p.LancamentoId == LancamentoId && p.Status != StatusParcela.Pago &&
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
            return [];
        }
    }


    // Atualiza a parcela do lançamento
    public async Task<bool> UpdateLancamentoParcela(LancamentoParcela parcela)
    {
        try
        {
            _db.LancamentoParcelas.Update(parcela);
            await _db.SaveChangesAsync();
            return true;

        }
        catch
        {
            return false;
        }
    }

    // ## Busca a parcela
    public async Task<LancamentoParcela?> BuscaLancamentoParcelasync(int id)
    {
        try
        {
            return await _db.LancamentoParcelas.FindAsync(id);
        }
        catch
        {
            return null;
        }

    }

}