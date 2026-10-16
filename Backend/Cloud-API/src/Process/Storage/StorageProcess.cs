using Cloud_API.src.DTO;

namespace Cloud_API.src.Process.Storage
{
    public class StorageProcess : IStorageProcess
    {
        private readonly string RepositoryBase;

        public StorageProcess(IConfiguration configuration)
        {
            RepositoryBase = configuration.GetSection("LocalSettings:PathStrings:BasePathRepositoryBase").Value;
        }

        public async Task<bool> DeleteDocumento(DocumentoDTO arquivo)
        {
            string DocumentPath = Path.Combine(RepositoryBase, arquivo.NameFile + arquivo.extension);

            try
            {
                if (File.Exists(DocumentPath))
                {
                    File.Delete(DocumentPath);
                }

                return true;
            }
            catch
            {
                return false;
            }          
        }
    }
}
