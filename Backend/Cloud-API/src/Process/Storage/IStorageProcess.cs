using Cloud_API.src.DTO;

namespace Cloud_API.src.Process.Storage
{
    public interface IStorageProcess
    {
        public Task<bool> DeleteDocumento(DocumentoDTO arquivo);
    }
}
