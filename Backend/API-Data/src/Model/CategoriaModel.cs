using API_Data.src.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Data.src.Model
{
    public class Categoria
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public Atribuicao Atribuicao { get; set; }

        // Coleções para navegação inversa
        // Coleções para bater com o .WithMany(...)
        public ICollection<Lancamento> Lancamentos { get; set; } = new List<Lancamento>();
        public ICollection<ContaFixa> ContasFixas { get; set; } = new List<ContaFixa>();
    }
}
