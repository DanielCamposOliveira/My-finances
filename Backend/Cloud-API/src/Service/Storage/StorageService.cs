using Cloud_API.src.Background.BackgroundProcess;
using Cloud_API.src.DTO;
using Cloud_API.src.Process.Storage;
using Cloud_API.src.Repository.DataBase;
using Cloud_API.src.Repository.Dictionary;

namespace Cloud_API.src.Service.Storage
{
    public class StorageService : IStorageService
    {
        private readonly DictionaryRepository BancoDadosMemoria;

        private readonly IFilaDeTrabalho _fila;

        private readonly IStorageProcess _storageProcess;
        private readonly IDataBaseRepository BancoDadosLocal;

        public StorageService(IStorageProcess storageProcess, IDataBaseRepository dataBaseRepository, DictionaryRepository dictionaryRepository, IFilaDeTrabalho fila)
        {
            _storageProcess = storageProcess;
            BancoDadosLocal = dataBaseRepository;

            BancoDadosMemoria = dictionaryRepository;
            _fila = fila;
        }

        public async Task<IResult> Delete(string token)
        {
            // ==========================================
            // 1. VERIFICA SE O ARQUIVO EXISTE NO BANCO DE DADOS
            // ==========================================
            DocumentoDownloadDTO dados = await BancoDadosMemoria.ObterRegistro(token);

            if (dados == null)
            {
                return Results.BadRequest();
            }


            // ==========================================
            // 3. APAGA OS DADOS EXISTE NO BANCO DE DADOS
            // ==========================================

            //REGISTRA NO BANCO DE DADOS (VIA FILA) - Delete
            await _fila.EnfileirarTarefaAsync(token => BancoDadosLocal.Delete(dados.NameArchived));

            //REGISTRA NO BANCO DE MEMORIA - Delete
            await BancoDadosMemoria.Delete(dados.NameArchived);


            // monta o pacote
            DocumentoDTO Pacote = new DocumentoDTO
            {
                NameFile = dados.NameArchived,
                extension = dados.Extension
            };

            // apaga o arquivo no disco
            bool ApagarNoDisco = await _storageProcess.DeleteDocumento(Pacote);
            //if (ApagarNoDisco == false)
            //{
            //    return Results.BadRequest();
            //}
                       

            return Results.Ok();
        }
    }
}
