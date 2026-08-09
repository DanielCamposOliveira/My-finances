namespace API_Data.src.Model
{
    public class Tag
    {

        public int Id { get; set; }   
        public string Nome { get; set; } = string.Empty;

        // Relacionamento Obrigatório com User
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        // Coleções para navegação inversa
        // Coleções para bater com o .WithMany(...)
        public ICollection<Lancamento> Lancamentos { get; set; } = new List<Lancamento>();
        public ICollection<ContaFixa> ContasFixas { get; set; } = new List<ContaFixa>();
    }
}

