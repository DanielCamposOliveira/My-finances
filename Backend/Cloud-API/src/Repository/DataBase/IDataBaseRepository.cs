using Cloud_API.src.DTO;

namespace Cloud_API.src.Repository.DataBase
{
    public interface IDataBaseRepository
    {
        public Task<bool> RegistraArquivo(UploadChunkDTO Dados, string NomeGerado);

        public Task<DocumentoDownloadDTO> ObterRegistro(string token);

        public Task<bool> UpdateDateAccessed(string NameArchived);

        public Task<bool> Delete(string NameArchived);

        public Task<bool> Active(string NameArchived, bool valor);
    }
}
