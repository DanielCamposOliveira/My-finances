using API_Data.src.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Data.src.Model
{
    public class Categoria
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public Atribuicao Atribuicao { get; set; }

        public ICollection<Lancamento> Lancamentos { get; set; } = new List<Lancamento>();
    }
}
