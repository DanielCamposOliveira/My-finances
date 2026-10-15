namespace Cloud_API.src.Service.Storage
{
    public interface IStorageService
    {
        public Task<IResult> Delete(string token);
    }
}
