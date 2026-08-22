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
    public async Task<IResult> ListarLancamentosAsync(string userId)
    {
        var retorno = await _repository.ListaTodosLancamentosAsync(userId);
        if (retorno == null)
        {
            return Results.Problem(
            "Ocorreu um Erro ao Listar Lançamentos",
            statusCode: StatusCodes.Status500InternalServerError);
        }

        return Results.Ok(retorno);
    }

    //## Obter faturas com Status Aberto(Mes recorent) ou Vencida(Ano recorrent) 
 
    public async Task<IResult> ListFaturaPendenteAsync(string userId)
    {
        var LancamentosAtivos = await _repository.ListaLancamentosAsync(userId);
        if (LancamentosAtivos == null)
        {
            return Results.Problem(
            "Ocorreu um Erro ao Listar Faturas Pendente",
            statusCode: StatusCodes.Status500InternalServerError);
        }

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
                    descricao = parcela.Lancamento.Descricao,
                    dependence_id = parcela.LancamentoId,
                    Atribuicao = parcela.Lancamento.Categoria.Atribuicao,
                });
            }
        }

        return Results.Ok(faturasGeradas);
    }

    // Cria um lançamento
    public async Task<IResult> CriarLancamentoAsync(Create dto, string userId)
    {
        // Verifica se existe a categoria
        var categoriaExiste = await _repository.CategoriaExisteAsync(dto.CategoriaId, userId);

        if (!categoriaExiste)
        {
            return Results.Problem(
                "A categoria informada não existe.",
                statusCode: StatusCodes.Status404NotFound);
        }

        // Busca das tags informadas
        var ListTags = await _repository.ObterTagsPorIdsAsync(dto.TagIds, userId);

        // Definição da quantidade de parcelas
        // int quantidadeEfetivaParcelas = dto.Tipo == TipoLancamento.Parcelado ? dto.QtdParcelas : 1;

        // Mapeamento para a entidade do domínio
        var lancamento = new Lancamento
        {
            Descricao = dto.Descricao,
            ValorTotal = dto.ValorTotal,
            QtdParcelas = dto.QtdParcelas,
            CategoriaId = dto.CategoriaId,
            UserId = userId,
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


    // Atualiza o status de uma parcela se pertencer ao usuario
    public async Task<IResult> UptateStatusLancamentoParcela(ParcelaUpdateStatus dto, string userId)
    {

        // Busca Parcela do Lancamento
        var parcela = await _repository.BuscaLancamentoParcelasync(dto.ParcelaId);
        if (parcela == null)
        {
            return Results.Problem(
                "Parcela não encontrada !",
                statusCode: StatusCodes.Status404NotFound
                );
        }

        // Busca Lancamento
        var _lancamento = await _repository.BuscaLancamentoasync(parcela.LancamentoId);
        if (_lancamento == null)
        {
            return Results.Problem(
                "Lancamento não encontrada !",
                statusCode: StatusCodes.Status404NotFound
                );
        }

        // verifica se o Lancamento é do Usuario
        if(_lancamento.UserId != userId)
        {
            return Results.Problem(
                "Parcela não Pertence ao usuario !",
                 statusCode: StatusCodes.Status403Forbidden
                );
        }


        // Altera dados da parcela
        parcela.Status = dto.Status;
        parcela.DataPagamento = dto.Status == StatusParcela.Pago ? DateTime.UtcNow : null;

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