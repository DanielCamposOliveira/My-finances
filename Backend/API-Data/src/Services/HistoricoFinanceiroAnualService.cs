using API_Data.src.DTOs;
using API_Data.src.Repository.Interface;
using API_Data.src.Services.Interface;

namespace API_Data.src.Services
{
    public class HistoricoFinanceiroAnualService : IHistoricoFinanceiroAnualService
    {
        private readonly IHistoricoFinanceiroAnualRepository _Repo;
        public HistoricoFinanceiroAnualService(IHistoricoFinanceiroAnualRepository repo)
        {
            _Repo = repo;
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

        public async Task<IResult> AtualizarHistoricoMesAsync(HistoricoMesRequest request)
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
    }
}
