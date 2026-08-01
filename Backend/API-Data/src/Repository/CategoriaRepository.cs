using API_Data.src.Data;
using API_Data.src.DTOs;
using API_Data.src.Model;
using Microsoft.EntityFrameworkCore;

namespace API_Data.src.Repository
{
    public class CategoriaRepository
    {
        private readonly AppDbContext _db;
        public CategoriaRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<CategoriaResponseDto?> CriarCategoriaAsync(Categoria categoria)
        {
            try
            {
                // Salva a entidade completa no banco
                _db.Categorias.Add(categoria);
                await _db.SaveChangesAsync();

               
                return new CategoriaResponseDto
                {
                    Nome = categoria.Nome,
                    Atribuicao = categoria.Atribuicao
                };

            }
            catch
            {
                return null;
            }
        }

 
        public async Task<List<CategoriaResponseDto>> ListaCategoriaAsync()
        {
            try
            {
                var categorias = await _db.Categorias
                    .AsNoTracking() // Dica: excelente para performance em consultas de leitura
                    .Select(c => new CategoriaResponseDto
                    {
                        Nome = c.Nome,
                        Atribuicao = c.Atribuicao
                    })
                    .ToListAsync();

                return categorias;
            }
            catch
            {
                return [];
            }
        }


    }
}
