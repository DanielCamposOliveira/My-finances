
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

        // # Cria conta fixa
        public async Task<IResult> CriarContaFixaAsync(CriarContaFixaDto Dados)
        {
            // Verifica se existe a categoria
            var categoriaExiste = await _repository.CheckCategoriasPorIdsAsync(Dados.CategoriaId);

            if (!categoriaExiste)
            {
                throw new ArgumentException("A categoria informada não existe.");
            }

            // Busca das tags informadas
            var ListTags = await _repository.ListaTagsPorIdsAsync(Dados.TagIds);

            // Mapeamento para a entidade do domínio
            var ContaFixaModel = new ContaFixa
            {
                Descricao = Dados.Descricao,
                ValorBase = Dados.ValorBase,
                DiaVencimento = Dados.DiaVencimento,
                CategoriaId = Dados.CategoriaId,
                Ativo = true,
                Tags = ListTags
            };

            // Salva no banco de dados
            var retorno =  await _repository.CriarContaFixaAsync(ContaFixaModel);

            if (retorno == null)
            {
                return Results.Problem(
                    "Erro ao cadastrar a conta fixa.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }

            return Results.Created();
        }


        // ## Gera Parcelas do Mes Atual
        public async Task<IResult> GerarFaturasMesAsync()
        {
            bool created = false;

            int ano = DateTime.Today.Year;
            int mes = DateTime.Today.Month;

            var contasAtivas = await _repository.ListaContasFixasAtivasAsync();
            

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
                    if(parcelaExistente !=null)
                    {
                        created = true;
                    }
                    
                }        
            }

            if (created == true)
                return Results.Created();


            return Results.Ok();
        }



        //## Obter faturas com Status Aberto(Mes recorent) ou Vencida(Ano recorrent) 
        public async Task<List<FaturaMesResponseDto>> ListFaturaPendenteAsync()
        {
            var contasAtivas = await _repository.ListaContasFixasAsync();
            var faturasGeradas = new List<FaturaMesResponseDto>();
            int ano = DateTime.Today.Year;
            int mes = DateTime.Today.Month;

            foreach (var conta in contasAtivas)
            {
                // 1. Busca no banco se já existem parcelas abertas/atrasadas para essa conta (do mês atual ou anteriores)
                var parcelasExistentes = await _repository.ListParcelasAbertasAtrasadasAsync(conta.Id, ano, mes);

                // Monta o DTO
                foreach (var parcela in parcelasExistentes)
                {
                    faturasGeradas.Add(new FaturaMesResponseDto(
                        parcela.Id,
                        conta.Id,
                        conta.Descricao,
                        parcela.ValorParcela,
                        parcela.DataVencimento,
                        parcela.DataPagamento,
                        parcela.Status
                    ));
                }
            }

            return faturasGeradas;
        }



        //## Atualiza o status da Fatura
        public async Task<IResult> UpdateStatusParcela(int parcelaId, StatusParcela status)
        {
            // Busca a parcela
            var parcela = await _repository.ObterParcelaPorIdAsync(parcelaId);

            if (parcela == null)
            { 
                return Results.Problem(
                "Parcela não encontrada !",
                statusCode: StatusCodes.Status404NotFound
                );
            }

            // Altera o status e registra o momento do pagamento
            parcela.Status = status;
            parcela.DataPagamento = DateTime.UtcNow;

            //Grava no banco de dados
            var retorno = await _repository.AtualizarStatusParcelaAsync(parcela);

            if(!retorno)
            {
                return Results.Problem(
                "Erro ao tentar atualizar o Status",
                statusCode: StatusCodes.Status500InternalServerError
                );
            }

            return Results.Created();
        }


        //## Atualiza o status da Fatura
        public async Task<IResult> UpdateStatusContaFixa(int Id, bool status)
        {
            // Busca a parcela
            var Conta = await _repository.ObterContaFixaPorIdAsync(Id);

            if (Conta == null)
            {
                return Results.Problem(
                "Parcela não encontrada !",
                statusCode: StatusCodes.Status404NotFound
                );
            }

            // Altera o status e registra o momento do pagamento
            Conta.Ativo = status;


            //Grava no banco de dados
            var retorno = await _repository.AtualizarStatusContaFixaAsync(Conta);

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
