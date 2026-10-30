using API_Data.src.DTOs.Result;

namespace API_Data.src.Services.Interface
{
    public interface IArquivosService
    {
        public Task<IResult> Upload();

        public Task<OperationResult> Download(string token);
    }
}
