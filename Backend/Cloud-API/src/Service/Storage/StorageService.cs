using Cloud_API.src.DTO;
using Cloud_API.src.Process.Storage;
using Cloud_API.src.Repository.DataBase;

namespace Cloud_API.src.Service.Storage
{
    public class StorageService : IStorageService
    {
        private readonly IStorageProcess _storageProcess;
        private readonly IDataBaseRepository _dataBaseRepository;

        public StorageService(IStorageProcess storageProcess, IDataBaseRepository dataBaseRepository)
        {
            _storageProcess = storageProcess;
            _dataBaseRepository = dataBaseRepository;
        }

        public async Task<IResult> Delete(string token)
        {
            // verifica se esse ARQUIVO existe no banco de dados
            DocumentoDownloadDTO dados = await _dataBaseRepository.ObterRegistro(token);
            if (dados == null)
            {
                return Results.NotFound();
            }

            // monta o pacote
            DocumentoDTO Pacote = new DocumentoDTO
            {
                FileName = dados.NameArchived,
                extension = dados.Extension
            };

            // apaga o arquivo no disco
            bool ApagarNoDisco = await _storageProcess.DeleteDocumento(Pacote);
            if (ApagarNoDisco == false)
            {
                return Results.BadRequest();
            }

            // apagar registro do bando de dados
            bool ApagarNoBD = await _dataBaseRepository.Delete(dados.NameArchived);
            if (ApagarNoBD == false)
            {
                return Results.BadRequest();
            }

            return Results.Ok();
        }
    }
}
