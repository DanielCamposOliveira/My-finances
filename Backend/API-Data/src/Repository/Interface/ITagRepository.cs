using API_Data.src.DTOs;
using API_Data.src.Model;

namespace API_Data.src.Repository.Interface
{
    public interface ITagRepository
    {
        public Task<Tag> CriarTag(Tag _tag);

        public Task<bool> CriarListTag(List<Tag> _tag);

        public Task<List<TagResponseDto>> ListaTags();
    }
}
