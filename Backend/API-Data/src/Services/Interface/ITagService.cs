using API_Data.src.DTOs;

namespace API_Data.src.Services.Interface
{
    public interface ITagService
    {
        public Task<IResult> CriarTag(CriarTagDto tag, string userId);

        public Task<IResult> ListaTags(string userId);

    }
}
