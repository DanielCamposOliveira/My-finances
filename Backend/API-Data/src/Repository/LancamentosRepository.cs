using API_Data.src.Data;
using API_Data.src.Model;
using Microsoft.EntityFrameworkCore;
using static API_Data.src.DTOs.LancamentoDto;

namespace API_Data.src.Repository;

public class LancamentosRepository
{
    private readonly AppDbContext _db;

    public LancamentosRepository(AppDbContext db)
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

    public async Task<string?> ObterNomeCategoriaAsync(int categoriaId)
    {
        var categoria = await _db.Categorias.FindAsync(categoriaId);
        return categoria?.Nome;
    }

    public async Task<Lancamento> AdicionarLancamentoAsync(Lancamento lancamento)
    {
        _db.Lancamentos.Add(lancamento);
        await _db.SaveChangesAsync();
        return lancamento;
    }


    public async Task<List<LancamentoResponseDto>> GetLancamentosAsync()
    {
        try
        {

            return await _db.Lancamentos
                .AsNoTracking()
                .Select(l => new LancamentoResponseDto(
                    l.Id,
                    l.Descricao,
                    l.ValorTotal,
                    l.Tipo,
                    l.QtdParcelas,
                    l.Categoria.Nome,
                    l.Tags.Select(t => t.Nome).ToList(),
                    l.Parcelas.Select(p => new ParcelaResponseDto(
                        p.Id, p.NumeroParcela, p.ValorParcela, p.DataVencimento, p.DataPagamento, p.Status
                    )).ToList()
                ))
                .ToListAsync();
        }
        catch
        {
            return new List<LancamentoResponseDto> { };
        }
    }
}