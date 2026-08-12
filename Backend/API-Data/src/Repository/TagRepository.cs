using API_Data.src.Data;
using API_Data.src.DTOs;
using API_Data.src.Model;
using API_Data.src.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace API_Data.src.Repository
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _db;
        public TagRepository(AppDbContext db)
        {
            _db = db;
        }

        // Cria Tag
        public async Task<Tag> CriarTag(Tag _tag)
        {
            try
            {               
                _db.Tags.AddAsync(_tag);
                await _db.SaveChangesAsync();

                return _tag;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CriarListTag(List<Tag> _tag)
        {
            try
            {
                _db.Tags.AddRangeAsync(_tag);
                await _db.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<TagResponseDto>> ListaTags()
        {
            try
            {
                var tags = await _db.Tags
                    .AsNoTracking() // Dica: excelente para performance em consultas de leitura
                    .Select(c => new TagResponseDto
                    {
                        Id = c.Id,
                        Nome = c.Nome
                    })
                    .ToListAsync();

                return tags;
            }
            catch
            {
                return null;
            }

        }

    }
}
