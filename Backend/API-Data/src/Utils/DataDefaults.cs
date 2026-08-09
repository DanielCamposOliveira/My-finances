using API_Data.src.Enum;
using API_Data.src.Model;

namespace API_Data.src.Utils
{
    // Pacote de registros padrao para cada Usuario criado

    public static class CategoriaDefaults
    {
        public static List<Categoria> ObterCategoriasPadrao(string userId)
        {
            return new List<Categoria>
            {
                new Categoria { Nome = "Moradia", Atribuicao = Atribuicao.Despesa, UserId = userId },
                new Categoria { Nome = "Transporte", Atribuicao = Atribuicao.Despesa, UserId = userId },
                new Categoria { Nome = "Alimentação", Atribuicao = Atribuicao.Despesa, UserId = userId },
                new Categoria { Nome = "Lazer", Atribuicao = Atribuicao.Despesa, UserId = userId },
                new Categoria { Nome = "Educação", Atribuicao = Atribuicao.Despesa, UserId = userId },
                new Categoria { Nome = "Salário", Atribuicao = Atribuicao.Ganho, UserId = userId },
                new Categoria { Nome = "Investimentos", Atribuicao = Atribuicao.Despesa, UserId = userId },
                new Categoria { Nome = "Outros", Atribuicao = Atribuicao.Despesa, UserId = userId },
                new Categoria { Nome = "Vale-Refeição", Atribuicao = Atribuicao.Ganho, UserId = userId },
                new Categoria { Nome = "Vale-Transporte", Atribuicao = Atribuicao.Ganho, UserId = userId }
            };
        }
    }
        
    public static class TagDefaults
    {
        public static List<Tag> ObterTagsPadrao(string userId)
        {

            return new List<Tag>
            {
                new Tag { Nome = "Casa", UserId = userId },
                new Tag { Nome = "Carro", UserId = userId },
                new Tag { Nome = "Gastos", UserId = userId },
                new Tag { Nome = "Mercado", UserId = userId },
                new Tag { Nome = "Energia", UserId = userId },
                new Tag { Nome = "Agua", UserId = userId },
                new Tag { Nome = "Internet", UserId = userId },
                new Tag { Nome = "Pix", UserId = userId },
                new Tag { Nome = "Faculdade", UserId = userId },
                new Tag { Nome = "Emprestimo", UserId = userId },
                new Tag { Nome = "Streaming", UserId = userId }
            };
        }
    }
}
