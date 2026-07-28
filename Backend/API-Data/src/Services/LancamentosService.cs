using API_Data.src.Enum;
using API_Data.src.Model;
using API_Data.src.Repository;
using static API_Data.src.DTOs.LancamentoDto;

namespace API_Data.src.Services;

public class LancamentosService
{
    private readonly LancamentosRepository _repository;

    public LancamentosService(LancamentosRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<LancamentoResponseDto>> ObterTodosLancamentosAsync()
    {
        return await _repository.GetLancamentosAsync();
    }


    public async Task<LancamentoResponseDto> CriarLancamentoAsync(CriarLancamentoDto dto)
    {
        // 1. Validação de existência da categoria
        var categoriaExiste = await _repository.CategoriaExisteAsync(dto.CategoriaId);
        if (!categoriaExiste)
        {
            throw new ArgumentException("A categoria informada não existe.");
        }

        // 2. Busca das tags informadas
        var tags = await _repository.ObterTagsPorIdsAsync(dto.TagIds);

        // 3. Definição da quantidade de parcelas
        int quantidadeEfetivaParcelas = dto.Tipo == TipoLancamento.Parcelado ? dto.QtdParcelas : 1;

        var lancamento = new Lancamento
        {
            Descricao = dto.Descricao,
            ValorTotal = dto.ValorTotal,
            Tipo = dto.Tipo,
            QtdParcelas = quantidadeEfetivaParcelas,
            CategoriaId = dto.CategoriaId,
            Tags = tags
        };

        // 4. Cálculo e geração automática das parcelas
        decimal valorCalculadoParcela = Math.Round(dto.ValorTotal / quantidadeEfetivaParcelas, 2);

        for (int i = 0; i < quantidadeEfetivaParcelas; i++)
        {
            lancamento.Parcelas.Add(new Parcela
            {
                NumeroParcela = i + 1,
                ValorParcela = valorCalculadoParcela,
                DataVencimento = dto.DataPrimeiroVencimento.AddMonths(i),
                Status = StatusParcela.Aberto
            });
        }

        // 5. Persistência dos dados
        await _repository.AdicionarLancamentoAsync(lancamento);

        var nomeCategoria = await _repository.ObterNomeCategoriaAsync(dto.CategoriaId);

        // 6. Mapeamento de retorno para o DTO
        return new LancamentoResponseDto(
            lancamento.Id,
            lancamento.Descricao,
            lancamento.ValorTotal,
            lancamento.Tipo,
            lancamento.QtdParcelas,
            nomeCategoria ?? string.Empty,
            tags.Select(t => t.Nome).ToList(),
            lancamento.Parcelas.Select(p => new ParcelaResponseDto(
                p.Id, p.NumeroParcela, p.ValorParcela, p.DataVencimento, p.DataPagamento, p.Status
            )).ToList()
        );
    }












}