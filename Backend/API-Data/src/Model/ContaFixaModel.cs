namespace API_Data.src.Model
{
    public class ContaFixa
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal ValorBase { get; set; }
        public int DiaVencimento { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;


        // Relacionamento com User
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;

        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<ContaFixaParcela> Parcelas { get; set; } = new List<ContaFixaParcela>();
    }
}
