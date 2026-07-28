
using API_Data.src.DTOs;
using API_Data.src.Enum;
using API_Data.src.Model;
using API_Data.src.Repository;

namespace API_Data.src.Services
{
    public class ContasFixasService
    {
        private readonly ContasFixasRepository _repository;

        public ContasFixasService(ContasFixasRepository repository)
        {
            _repository = repository;
        }
        public async Task<ContaFixaResponseDto> CriarContaFixaAsync(CriarContaFixaDto dto)
        {
            // 1. Validação de existência da categoria
            var categoriaExiste = await _repository.CategoriaExisteAsync(dto.CategoriaId);
            if (!categoriaExiste)
            {
                throw new ArgumentException("A categoria informada não existe.");
            }

            // 2. Busca das tags informadas
            var tags = await _repository.ObterTagsPorIdsAsync(dto.TagIds);

            // 3. Mapeamento para a entidade do domínio
            var contaFixa = new ContaFixa
            {
                Descricao = dto.Descricao,
                ValorBase = dto.ValorBase,
                DiaVencimento = dto.DiaVencimento,
                CategoriaId = dto.CategoriaId,
                Ativo = true,
                Tags = tags
            };

            // 4. Persistência
            await _repository.AdicionarContaFixaAsync(contaFixa);

            var nomeCategoria = await _repository.ObterNomeCategoriaAsync(dto.CategoriaId);

            // 5. Retorno do DTO
            return new ContaFixaResponseDto(
                contaFixa.Id,
                contaFixa.Descricao,
                contaFixa.ValorBase,
                contaFixa.DiaVencimento,
                contaFixa.Ativo,
                nomeCategoria ?? string.Empty,
                tags.Select(t => t.Nome).ToList()
            );
        }

        public async Task<List<FaturaMesResponseDto>> ObterOuGerarFaturasDoMesAsync(int ano, int mes)
        {
            var contasAtivas = await _repository.ObterContasFixasAtivasAsync();
            var faturasGeradas = new List<FaturaMesResponseDto>();

            foreach (var conta in contasAtivas)
            {
                var parcelaExistente = await _repository.ObterParcelaDoMesAsync(conta.Id, ano, mes);

                if (parcelaExistente == null)
                {
                    // Regra para gerar o vencimento correto no mês/ano solicitados
                    int diaAjustado = Math.Min(conta.DiaVencimento, DateTime.DaysInMonth(ano, mes));
                    var dataVencimento = new DateTime(ano, mes, diaAjustado, 0, 0, 0, DateTimeKind.Utc);

                    var novaParcela = new Parcela
                    {
                        ContaFixaId = conta.Id,
                        NumeroParcela = mes,
                        ValorParcela = conta.ValorBase,
                        DataVencimento = dataVencimento,
                        Status = StatusParcela.Aberto
                    };

                    parcelaExistente = await _repository.CriarParcelaFixaAsync(novaParcela);
                }

                faturasGeradas.Add(new FaturaMesResponseDto(
                    parcelaExistente.Id,
                    conta.Id,
                    conta.Descricao,
                    parcelaExistente.ValorParcela,
                    parcelaExistente.DataVencimento,
                    parcelaExistente.DataPagamento,
                    parcelaExistente.Status
                ));
            }

            return faturasGeradas;
        }
    }
}
