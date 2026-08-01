namespace API_Data.src.Model
{
    public class Tag
    {

        public int Id { get; set; }   
        public string Nome { get; set; } = string.Empty;


        // Coleções para navegação inversa
        public ICollection<Lancamento> Lancamentos { get; set; } = new List<Lancamento>();
        public ICollection<ContaFixa> ContasFixas { get; set; } = new List<ContaFixa>();
    }
}

