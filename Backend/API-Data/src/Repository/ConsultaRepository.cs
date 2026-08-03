using API_Data.src.Data;
using API_Data.src.Enum;
using API_Data.src.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace API_Data.src.Repository
{
    public class ConsultaRepository : IConsultaRepository
    {
        private readonly AppDbContext _db;
        public ConsultaRepository(AppDbContext db)
        {
            _db = db;
        }

 


        #region Rotas validadas


        /// <summary>
        /// Calcula o valor total do campo "ValorParcela" das tabelas "ContaFixaParcelas" e "LancamentoParcelas" 
        /// Que estejam com status "Aberto"
        /// Que tenham atribuição "Ganho"
        /// RASCUNHO: Soma tudo que estejam com vencimento até o mês/ano informado e seja com status "Aberto" e atribuição "Ganho"
        /// </summary>
        public async Task<decimal> TotalReceber(int ano, int mes)
        {
            try
            {
                // Define o primeiro dia do próximo mês para filtrar vencimentos anteriores a ele
                var limiteData = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1);

                // Query 1: Contas Fixas a Receber
                var fixas = _db.ContaFixaParcelas
                    .AsNoTracking()
                    .Where(cfp => cfp.Status == StatusParcela.Aberto
                               && cfp.ContaFixa.Categoria.Atribuicao == Atribuicao.Ganho
                               && cfp.DataVencimento < limiteData)
                    .Select(cfp => (decimal?)cfp.ValorParcela);

                // Query 2: Lançamentos Variáveis a Receber
                var variaveis = _db.LancamentoParcelas
                    .AsNoTracking()
                    .Where(lp => lp.Status == StatusParcela.Aberto // Fix: alterado de Pago para Aberto
                              && lp.Lancamento.Categoria.Atribuicao == Atribuicao.Ganho
                              && lp.DataVencimento < limiteData)
                    .Select(lp => (decimal?)lp.ValorParcela);

                // Une as consultas com UNION ALL no SQL e realiza a soma no banco
                var total = await fixas.Concat(variaveis).SumAsync();

                return total ?? 0m;
            }
            catch (Exception)
            {
                return 0m;
            }
        }


        /// <summary>
        /// Calcula o valor total dos campo "ValorParcela" das tabelas "ContaFixaParcelas", "LancamentoParcelas" 
        /// Que estejam com status "Pago"
        /// Que tenham atribuição "Ganho"
        /// Que a DataPagamento estejam dentro do mês e ano especificados.
        /// RASCUNHO: Isso significa que vai buscar todas as dividas que foram pagas no mês
        /// </summary>
        public async Task<Decimal> TotalSaldo(int ano, int mes)
        {
            try
            {
                // Define o início do mês (ex: 01/03/2026 00:00:00)
                var inicioMes = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Utc);

                // Define o início do próximo mês (ex: 01/04/2026 00:00:00)
                var fimMes = inicioMes.AddMonths(1);

                // Query 1: Contas Fixas pagas do mês
                var fixas = _db.ContaFixaParcelas
                    .AsNoTracking()
                    .Where(cfp => cfp.Status == StatusParcela.Pago
                               && cfp.ContaFixa.Categoria.Atribuicao == Atribuicao.Ganho
                               && cfp.DataPagamento >= inicioMes
                               && cfp.DataPagamento < fimMes)
                    .Select(cfp => (decimal?)cfp.ValorParcela);

                // Query 2: Lançamentos pagos do mês
                var variaveis = _db.LancamentoParcelas
                    .AsNoTracking()
                    .Where(lp => lp.Status == StatusParcela.Pago // Alterado para buscar apenas PAGO
                              && lp.Lancamento.Categoria.Atribuicao == Atribuicao.Ganho
                              && lp.DataPagamento >= inicioMes
                              && lp.DataPagamento < fimMes)
                    .Select(lp => (decimal?)lp.ValorParcela);

                // Une os dois conjuntos e realiza a soma no banco de dados
                var total = await fixas.Concat(variaveis).SumAsync();

                return total ?? 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }


        /// <summary>
        /// Calcula o valor total do campo "ValorParcela" das tabelas "ContaFixaParcelas" e "LancamentoParcelas"
        /// que foram PAGAS e cujos vencimentos pertencem EXATAMENTE ao mês e ano informados.
        /// Desconsidera parcelas de meses anteriores (atrasadas) e meses futuros.
        /// </summary>
        /// RASCUNHO: Isso significa que vai buscar todas as dividas que foram criadas no mês, independente de estarem pagas ou não.
        public async Task<Decimal> TotalDividasMes(int ano, int mes)
        {
            try
            {
                // 1. Define o primeiro dia do mês atual (ex: 01/01/2026 00:00:00)
                var inicioMes = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Utc);

                // 2. Define o primeiro dia do próximo mês (ex: 01/02/2026 00:00:00)
                var fimMes = inicioMes.AddMonths(1);


                // Query 1: Contas Fixas Pagas no Mês
                var fixasPagas = _db.ContaFixaParcelas
                    .AsNoTracking()
                    .Where(cfp => cfp.ContaFixa.Categoria.Atribuicao == Atribuicao.Despesa
                               && cfp.DataVencimento >= inicioMes
                               && cfp.DataVencimento < fimMes)
                    .Select(cfp => (decimal?)cfp.ValorParcela);

                // Query 2: Lançamentos Variáveis Pagos no Mês
                var variaveisPagas = _db.LancamentoParcelas
                    .AsNoTracking()
                    .Where(lp => lp.Lancamento.Categoria.Atribuicao == Atribuicao.Despesa
                              && lp.DataVencimento >= inicioMes
                              && lp.DataVencimento < fimMes)
                    .Select(lp => (decimal?)lp.ValorParcela);

                // Une as duas consultas e soma diretamente no banco de dados
                var total = await fixasPagas.Concat(variaveisPagas).SumAsync();

                return total ?? 0m;
            }
            catch (Exception)
            {
                return 0m;
            }
        }


        /// <summary>
        /// Calcula o valor total do campo "ValorParcela" das tabelas "ContaFixaParcelas" e "LancamentoParcelas" 
        /// Que não  estejama com status "Pago"
        /// Que tenham atribuição "Despesa"
        /// Que estejam com vencimento até o mês/ano informado.
        /// RASCUNHO: Retorna o valor total das todas dividas que foram criadas no mês que esta em aberto e as contas Atrasado dos meses anteriores
        /// </summary>

        public async Task<Decimal> TotalContasPendentes(int ano, int mes)
        {
            try
            {
                // Garante a data sem fuso ou conversões complexas na expressão
                var limiteData = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1);

                // Query 1: Contas Fixas
                var fixas = _db.ContaFixaParcelas
                    .AsNoTracking()
                    .Where(cfp => cfp.Status != StatusParcela.Pago
                               && cfp.ContaFixa.Categoria.Atribuicao == Atribuicao.Despesa
                               && cfp.DataVencimento < limiteData)
                    .Select(cfp => (decimal?)cfp.ValorParcela); // Cast para decimal? evita exceção se não houver registros

                // Query 2: Lançamentos
                var variaveis = _db.LancamentoParcelas
                    .AsNoTracking()
                    .Where(lp => lp.Status != StatusParcela.Pago
                              && lp.Lancamento.Categoria.Atribuicao == Atribuicao.Despesa
                              && lp.DataVencimento < limiteData)
                    .Select(lp => (decimal?)lp.ValorParcela);

                // O Concat gera exatamente o UNION ALL do seu SQL
                var total = await fixas.Concat(variaveis).SumAsync();

                return total ?? 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }



        /// <summary>
        /// Calcula o valor total do campo "ValorParcela" das tabelas "ContaFixaParcelas" e "LancamentoParcelas"
        /// que foram PAGAS e cujos vencimentos pertencem EXATAMENTE ao mês e ano informados.
        /// Desconsidera parcelas de meses anteriores (atrasadas) e meses futuros.
        /// RASCUNHO: Isso significa que vai buscar todas as dividas que foram pagas no mês
        /// </summary>
        public async Task<Decimal> TotalQuitadasDoMes(int ano, int mes)
        {
            try
            {
                // 1. Define o primeiro dia do mês atual (ex: 01/01/2026 00:00:00)
                var inicioMes = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Utc);

                // 2. Define o primeiro dia do próximo mês (ex: 01/02/2026 00:00:00)
                var fimMes = inicioMes.AddMonths(1);


                // Query 1: Contas Fixas Pagas no Mês
                var fixasPagas = _db.ContaFixaParcelas
                    .AsNoTracking()
                    .Where(cfp => cfp.Status == StatusParcela.Pago
                               && cfp.ContaFixa.Categoria.Atribuicao == Atribuicao.Despesa
                               && cfp.DataPagamento >= inicioMes
                               && cfp.DataPagamento < fimMes)
                    .Select(cfp => (decimal?)cfp.ValorParcela);

                // Query 2: Lançamentos Variáveis Pagos no Mês
                var variaveisPagas = _db.LancamentoParcelas
                    .AsNoTracking()
                    .Where(lp => lp.Status == StatusParcela.Pago
                              && lp.Lancamento.Categoria.Atribuicao == Atribuicao.Despesa
                              && lp.DataPagamento >= inicioMes
                              && lp.DataPagamento < fimMes)
                    .Select(lp => (decimal?)lp.ValorParcela);

                // Une as duas consultas e soma diretamente no banco de dados
                var total = await fixasPagas.Concat(variaveisPagas).SumAsync();

                return total ?? 0m;
            }
            catch (Exception)
            {
                return 0m;
            }
        }



        #endregion

        //RASCUNHO
        // Forma mais neutra para evitar conversões de fuso desnecessárias no SQL
        //var limiteData = new DateTime(ano, mes, 1).AddMonths(1);





    }
}
