using System.ComponentModel.DataAnnotations.Schema;

namespace API_Data.src.Model
{
    public class Tag
    {

        public int Id { get; set; }   
        public string Nome { get; set; } = string.Empty;

        public ICollection<Lancamento> Lancamentos { get; set; } = new List<Lancamento>();
    }
}

