using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Data.src.Model
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsAdmin { get; set; } = false;


        // Relacionamentos inversos
        public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<ContaFixa> ContasFixas { get; set; } = new List<ContaFixa>();
        public ICollection<Lancamento> Lancamentos { get; set; } = new List<Lancamento>();
        public ICollection<HistoricoFinanceiroAnual> HistoricosFinanceiros { get; set; } = new List<HistoricoFinanceiroAnual>();
    }
}
