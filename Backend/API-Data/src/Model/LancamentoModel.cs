using API_Data.src.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Data.src.Model
{
    public class Lancamento
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public int QtdParcelas { get; set; } = 1; 
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
       
        // Relacionamento com User
        public string UserId { get; set; }
        public User User { get; set; } = null!;


        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<LancamentoParcela> Parcelas { get; set; } = new List<LancamentoParcela>();
    }
}
