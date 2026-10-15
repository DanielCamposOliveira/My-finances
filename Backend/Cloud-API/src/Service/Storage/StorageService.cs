using Cloud_API.src.Process.Storage;

namespace Cloud_API.src.Service.Storage
{
    public class StorageService
    {
        private readonly IStorageProcess _storageProcess;

        public StorageService(IStorageProcess storageProcess)
        {
            _storageProcess = storageProcess;
        }

        public async Task<IResult> Delete(string token)
        {

          //bool retorno = await _storageProcess.DeleteDocumento(token);

          //  if(retorno == false)
          //  {
          //      return Results.BadRequest();
          //  }

            return Results.Ok();

        }
    }
}
