using API_Data.src.DTOs;
using API_Data.src.Repository.Interface;
using API_Data.src.Services.Interface;

namespace API_Data.src.Services
{
    public class HistoricoFinanceiroAnualService : IHistoricoFinanceiroAnualService
    {
        private readonly IHistoricoFinanceiroAnualRepository _Repo;
        private readonly IConsultaService _service;
        public HistoricoFinanceiroAnualService(IHistoricoFinanceiroAnualRepository repo, IConsultaService service)
        {
            _Repo = repo;
            _service = service;
        }
        public async Task<List<GraficoHistoricoResponse>> ListaHistoricoAsync(int ano)
        {
            // Busca os registros existentes do ano no banco
            var registrosBanco = await _Repo.ObterTodosHistoricosAsync(ano);

            // Prepara os arrays de 12 posições zerados
            var listaSaldo = new decimal[12];
            var listaDivida = new decimal[12];

            //Preenche os meses que já possuem dados gravados
            foreach (var registro in registrosBanco)
            {
                int indiceMes = registro.Mes - 1; // Mês 1 vira índice 0
                if (indiceMes >= 0 && indiceMes < 12)
                {
                    listaSaldo[indiceMes] = registro.TotalSaldo;
                    listaDivida[indiceMes] = registro.TotalDivida;
                }
            }

            //Monta a estrutura igual ao seu exemplo de gráfico
            var Dados = new GraficoHistoricoResponse
            {
                ChartSeries = new List<SerieGrafico>
                {
                    new SerieGrafico
                    {
                        Type = "line",
                        Name = "Saldo",
                        Color = "#0097FF",
                        Data = listaSaldo.ToList()
                    },
                    new SerieGrafico
                    {
                        Type = "line",
                        Name = "Dívidas",
                        Color = "#E74C3C",
                        Data = listaDivida.ToList()
                    }
                }
            };

            return new List<GraficoHistoricoResponse> { Dados };

        }

        /// <summary>
        /// Atualiza o histórico do mês com os valores fornecidos no request.
        /// RASCUNHO: Este método é chamado para atualizar o histórico do mês com os valores fornecidos 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IResult> UpdateHistoricoMesAsync(HistoricoMesRequest request)
        {
           var response = await _Repo.AtualizarHistoricoMesAsync(request);
           if(response == false)
           {
                return Results.Problem(
                 "Erro ao cadastrar a conta fixa.",
                 statusCode: StatusCodes.Status500InternalServerError);
            }

            return Results.Created();

        }

        /// <summary>
        /// Gera o histórico do mês anterior, verificando se já existe um registro para o mês e ano correspondentes. 
        /// Se não existir, calcula o total de dívidas e saldo, e cria um novo registro no banco de dados.
        /// RASCUNHO: Este método é chamado para gerar o histórico do mês anterior, verificando se já existe um registro para o mês e ano correspondentes.
        /// </summary>
        /// <returns></returns>
        public async Task<IResult> GerarHistoricoMesAsync()
        {
     
            int ano = DateTime.Today.Year;
            int mes = DateTime.Today.Month - 1; // Pega o mes passado
            // verifica se o mês é 0 (janeiro), então ajusta para dezembro do ano anterior
            if (mes == 0)
            {
                mes = 12;
                ano--;
            }


            //verifica se já existe um registro para o mês atual
            var Valor = await _Repo.ObterHistoricosMesAsync(mes, ano);
            if (Valor != null && Valor.Any())
            {
                return Results.Conflict($"O histórico para {mes}/{ano} já existe.");
            }


            var _TotalDivida = await _service.TotalDividasMes();
            var _TotalSaldo = await _service.TotalSaldo();

            // Monta o pacote de dados para o histórico do mês
            var HistoricoMesRequest = new HistoricoMesRequest
            {   
                ano = Convert.ToInt32(ano),
                mes = Convert.ToInt32(mes),
                novoSaldo = Convert.ToInt32(_TotalSaldo),
                novaDivida = Convert.ToInt32(_TotalDivida)
            };

            var response = await _Repo.AtualizarHistoricoMesAsync(HistoricoMesRequest);
            if (response == false)
            {
                return Results.Problem(
                 "Erro ao cadastrar o histórico do mês.",
                 statusCode: StatusCodes.Status500InternalServerError);
            }
            return Results.Created();
        }
    }
}
