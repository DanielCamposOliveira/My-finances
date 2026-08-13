
using API_Data.src.DTOs.ContasFixas;
using API_Data.src.Enum;
using API_Data.src.Model;
using API_Data.src.Repository.Interface;
using API_Data.src.Services.Interface;

namespace API_Data.src.Services
{
    public class ContasFixasService : IContasFixasService
    {
        private readonly IContasFixasRepository _repository;

        public ContasFixasService(IContasFixasRepository repository)
        {
            _repository = repository;
        }

        // ### Cria conta fixa
        public async Task<IResult> CriarContaFixaAsync(Create Dados, string userId)
        {
            // Verifica se existe a categoria
            bool categoriaExiste = await _repository.CheckCategoriasPorIdsAsync(Dados.CategoriaId, userId);

            if (!categoriaExiste)
            {
                return Results.Problem(
                    "Categoria não pertence ao usuario",
                    statusCode: StatusCodes.Status403Forbidden
                    );
            }

            // Busca das tags informadas
            var ListTags = await _repository.ListaTagsPorIdsAsync(Dados.TagIds, userId);

            // Mapeamento para a entidade do domínio
            var ContaFixaModel = new ContaFixa
            {
                Descricao = Dados.Descricao,
                ValorBase = Dados.ValorBase,
                DiaVencimento = Dados.DiaVencimento,
                CategoriaId = Dados.CategoriaId,
                Ativo = true,
                Tags = ListTags,
                UserId = userId
            };

            // Salva no banco de dados
            var retorno = await _repository.CriarContaFixaAsync(ContaFixaModel);

            if (retorno == null)
            {
                return Results.Problem(
                    "Erro ao cadastrar a conta fixa.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            return Results.Created();
        }


        // ### Gera Parcelas do Mes Atual
        public async Task<IResult> GerarFaturasMesAsync(string userId)
        {
            bool created = false;

            int ano = DateTime.Today.Year;
            int mes = DateTime.Today.Month;

            var contasAtivas = await _repository.ListaContasFixasAtivasAsync(userId);
            if(contasAtivas == null)
            {
                return Results.Problem(
                    "Ocorreu um Erro ao lista Contas Fixas",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            foreach (var conta in contasAtivas)
            {
                var parcelaExistente = await _repository.ObterParcelaDoMesAsync(conta.Id, ano, mes);

                if (parcelaExistente == null)
                {
                    // Regra para gerar o vencimento correto no mês/ano solicitados
                    int diaAjustado = Math.Min(conta.DiaVencimento, DateTime.DaysInMonth(ano, mes));
                    var dataVencimento = new DateTime(ano, mes, diaAjustado, 0, 0, 0, DateTimeKind.Utc);

                    var novaParcela = new ContaFixaParcela
                    {
                        ContaFixaId = conta.Id,
                        NumeroParcela = mes,
                        ValorParcela = conta.ValorBase,
                        DataVencimento = dataVencimento,
                        Status = StatusParcela.Aberto
                    };

                    parcelaExistente = await _repository.CriarParcelaFixaAsync(novaParcela);
                    if (parcelaExistente != null)
                    {
                        created = true;
                    }

                }
            }

            if (created == true)
                return Results.Created();


            return Results.Ok();
        }


        // ### Lista todas as contasfixa
        public async Task<IResult> ListaTodasContasFixa(string userId)
        {
            var Contas = await _repository.ListaContasFixasAsync(userId);
            if(Contas == null)
            {
                return Results.Problem(
                    "Ocorreu um Erro ao lista Contas Fixas",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            var contasResponse = new List<ContaFixaResponse>();

            foreach (var _conta in Contas)
            {
                contasResponse.Add(new ContaFixaResponse
                {
                    Id = _conta.Id,
                    Descricao = _conta.Descricao,
                    ValorBase = _conta.ValorBase,
                    DiaVencimento = _conta.DiaVencimento,
                    CategoriaId = _conta.CategoriaId,      
                    Ativo = _conta.Ativo,
                    TagIds = _conta.Tags.Select(t => t.Id).ToList()   
                });
            }
            return Results.Ok(contasResponse);
        }


        //## Obter faturas com Status Aberto(Mes recorent) ou Vencida(Ano recorrent) 
        public async Task<IResult> ListFaturaPendenteAsync(string userId)
        {
            var contasAtivas = await _repository.ListaContasFixasAtivasAsync(userId);
            if (contasAtivas == null)
            {
                return Results.Problem(
                    "Ocorreu um Erro ao lista Contas Fixas",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            var faturasGeradas = new List<ParcelasResponse>();
            int ano = DateTime.Today.Year;
            int mes = DateTime.Today.Month;

            foreach (var conta in contasAtivas)
            {
                // 1. Busca no banco se já existem parcelas abertas/atrasadas para essa conta (do mês atual ou anteriores)
                var parcelasExistentes = await _repository.ListParcelasAbertasAtrasadasAsync(conta.Id, ano, mes);

                // Monta o DTO
                foreach (var parcela in parcelasExistentes)
                {
                    faturasGeradas.Add(new ParcelasResponse
                    {
                        Id = parcela.Id,
                        ContaFixaId = conta.Id,
                        Descricao = conta.Descricao,    
                        ValorParcela = parcela.ValorParcela,
                        DataVencimento = parcela.DataVencimento,
                        DataPagamento = parcela.DataPagamento,
                        Status = parcela.Status,
                        Atribuicao = parcela.ContaFixa.Categoria.Atribuicao
                    });
                }
            }

            return Results.Ok(faturasGeradas);
        }


        //### Atualiza o status da Fatura
        public async Task<IResult> UpdateStatusParcela(ParcelaUpdateStatus dto, string userId)
        {
            // obtem a parcela
            var parcela = await _repository.ObterParcelaAsync(dto.ParcelaId);
            if (parcela == null)
            {
                return Results.Problem(
                 "Parcela não encontrada",
                 statusCode: StatusCodes.Status404NotFound
                 );
            }

            // verifica se a conta pertence ao usuario
            bool checkConta = await _repository.ChecarContaFixa(parcela.ContaFixaId, userId);
            if (!checkConta)
            {
                return Results.Problem(
                   "Parcela não pertence ao usuario",
                   statusCode: StatusCodes.Status403Forbidden
                   );
            }

            // Altera dados
            parcela.Status = dto.Status;
            parcela.DataPagamento = dto.Status == StatusParcela.Pago ? DateTime.UtcNow : null;

            //Grava no banco de dados
            bool retorno = await _repository.UpdateParcelaAsync(parcela);

            if (!retorno)
            {
                return Results.Problem(
                "Erro ao tentar atualizar o ValorParcela",
                statusCode: StatusCodes.Status500InternalServerError
                );
            }

            return Results.Created();
        }


        //### Atualiza o ValorParcela da Fatura
        public async Task<IResult> UpdateValorParcela(ParcelaUpdateValor dto, string userId)
        {
            // obtem a parcela
            var parcela = await _repository.ObterParcelaAsync(dto.ParcelaId);
            if(parcela == null)
            {
                return Results.Problem(
                 "Parcela não encontrada",
                 statusCode: StatusCodes.Status404NotFound
                 );
            }

            // verifica se a conta pertence ao usuario
            bool checkConta = await _repository.ChecarContaFixa(parcela.ContaFixaId, userId);
            if(!checkConta)
            {
                return Results.Problem(
                   "Parcela não pertence ao usuario",
                   statusCode: StatusCodes.Status403Forbidden
                   );
            }

            // Altera dados
            parcela.ValorParcela = dto.ValorParcela;

            //Grava no banco de dados
            bool retorno = await _repository.UpdateParcelaAsync(parcela);

            if (!retorno)
            {
                return Results.Problem(
                "Erro ao tentar atualizar o ValorParcela",
                statusCode: StatusCodes.Status500InternalServerError
                );
            }

            return Results.Created();
        }


        //### Atualiza o status da ContaFixa
        public async Task<IResult> UpdateStatusContaFixa(ContaFixaUpdateStatus dto, string userId)
        {
            // Busca a parcela
            var Conta = await _repository.ObterContaFixaPorIdAsync(dto.Id_ContaFixa, userId);

            if (Conta == null)
            {
                return Results.Problem(
                       "Parcela não pertence ao usuario",
                       statusCode: StatusCodes.Status403Forbidden
                       );
            }

            // Altera dados
            Conta.Ativo = dto.Status;

            //Grava no banco de dados
            bool retorno = await _repository.AtualizarStatusContaFixaAsync(Conta);
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
}