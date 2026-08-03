using API_Data.src.DTOs.Lancamento;
using API_Data.src.Enum;
using API_Data.src.Model;
using API_Data.src.Repository.Interface;
using API_Data.src.Services.Interface;

namespace API_Data.src.Services;

public class LancamentosService : ILancamentosService
{
    private readonly ILancamentosRepository _repository;

    public LancamentosService(ILancamentosRepository repository)
    {
        _repository = repository;
    }

    //Lista todos os lançamentos
    public async Task<List<LancamentoResponse>> ListarLancamentosAsync()
    {
        var retorno = await _repository.ListaTodosLancamentosAsync();

        return retorno;
    }

    //## Obter faturas com Status Aberto(Mes recorent) ou Vencida(Ano recorrent) 
    public async Task<List<ParcelasResponse>> ListFaturaPendenteAsync()
    {
        var LancamentosAtivos = await _repository.ListaLancamentosAsync();
        var faturasGeradas = new List<ParcelasResponse>();
        int ano = DateTime.Today.Year;
        int mes = DateTime.Today.Month;

        foreach (var Lancamentos in LancamentosAtivos)
        {
            // 1. Busca no banco se já existem parcelas abertas/atrasadas para essa conta (do mês atual ou anteriores)
            var parcelasExistentes = await _repository.ListParcelasAbertasAtrasadasAsync(Lancamentos.Id, ano, mes);

            // Monta o DTO
            foreach (var parcela in parcelasExistentes)
            {
                faturasGeradas.Add(new ParcelasResponse
                {
                    Id = parcela.Id,
                    NumeroParcela = parcela.NumeroParcela,
                    ValorParcela = parcela.ValorParcela,
                    DataVencimento = parcela.DataVencimento,
                    Status = parcela.Status,
                    Lancamento_Descricao = parcela.Lancamento.Descricao,
                    Lancamento_Id = parcela.LancamentoId
                });
            }
        }

        return faturasGeradas;
    }

    // Cria um lançamento
    public async Task<IResult> CriarLancamentoAsync(Create dto)
    {
        // Verifica se existe a categoria
        var categoriaExiste = await _repository.CategoriaExisteAsync(dto.CategoriaId);

        if (!categoriaExiste)
        {
            return Results.Problem(
                "A categoria informada não existe.",
                statusCode: StatusCodes.Status404NotFound);
        }

        // Busca das tags informadas
        var ListTags = await _repository.ObterTagsPorIdsAsync(dto.TagIds);

        // Definição da quantidade de parcelas
        // int quantidadeEfetivaParcelas = dto.Tipo == TipoLancamento.Parcelado ? dto.QtdParcelas : 1;

        // Mapeamento para a entidade do domínio
        var lancamento = new Lancamento
        {
            Descricao = dto.Descricao,
            ValorTotal = dto.ValorTotal,
            QtdParcelas = dto.QtdParcelas,
            CategoriaId = dto.CategoriaId,
            Tags = ListTags
        };

        // Divide o valor total pelo numero de parcelas, a rredonda o resultado para 2 casas decimais
        decimal ValorParcela = Math.Round(dto.ValorTotal / dto.QtdParcelas, 2);

        for (int i = 0; i < dto.QtdParcelas; i++)
        {
            lancamento.Parcelas.Add(new LancamentoParcela
            {
                NumeroParcela = i + 1,
                ValorParcela = ValorParcela,
                DataVencimento = dto.DataPrimeiroVencimento.AddMonths(i),
                Status = StatusParcela.Aberto
            });
        }

        // Salva no banco de dados
        var retorno = await _repository.AdicionarLancamentoAsync(lancamento);

        if (retorno == null)
        {
            return Results.Problem(
                "Erro ao cadastrar a conta fixa.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        return Results.Created();
    }


    // Atualiza o status de uma parcela
    public async Task<IResult> UptateStatusLancamentoParcela(int id, StatusParcela status)
    {
        var parcela = await _repository.BuscaLancamentoParcelasync(id);
        if (parcela == null)
        {
            return Results.Problem(
            "Parcela não encontrada !",
            statusCode: StatusCodes.Status404NotFound
            );
        }

        // Altera dados
        parcela.Status = status;
        parcela.DataPagamento = status == StatusParcela.Pago ? DateTime.UtcNow : null;

        //Grava no banco de dados
        var retorno = await _repository.UpdateLancamentoParcela(parcela);

        if (!retorno)
        {
            return Results.Problem(
            "Erro ao tentar atualizar o Status",
            statusCode: StatusCodes.Status500InternalServerError
            );
        }

        return Results.Created();
    }


}