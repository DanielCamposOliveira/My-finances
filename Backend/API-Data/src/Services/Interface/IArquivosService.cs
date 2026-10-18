namespace API_Data.src.Services.Interface
{
    public interface IArquivosService
    {
        public Task<IResult> Upload();

        //public Task<IResult> Download(string token);
    }
}
