namespace API_Data.src.DTOs
{
    public class GraficoHistoricoResponse
    {
        public List<string> ChartCategories { get; set; } = new()
        {
            "Jan", "Fev", "Mar", "Abr", "Mai", "Jun",
            "Jul", "Ago", "Set", "Out", "Nov", "Dez"
        };

        public List<SerieGrafico> ChartSeries { get; set; } = new();
    }

    public class SerieGrafico
    {
        public string Type { get; set; } = "line";
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public List<decimal> Data { get; set; } = new();
    }

    public record HistoricoMesRequest
    {
        public int ano { get; init; }
        public int mes { get; init; }
        public int novoSaldo { get; init; }
        public int novaDivida { get; init; }
    }

}
