using Cloud_API.src.DTO;
using Cloud_API.src.Model;
using Microsoft.EntityFrameworkCore;

namespace Cloud_API.src.Repository.DataBase
{
    public class DataBaseRepository : IDataBaseRepository
    {
        private readonly AppDbContext _db;
        public DataBaseRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> RegistraArquivo(UploadChunkDTO Dados, string NomeGerado)
        {

            try
            {
                // verifica se existe algum arquivo com o mesmo nome, porem isso vai ser muito mas muito RARO
                if(await _db.StorageRegistry.AnyAsync(d => d.NameArchived == NomeGerado))
                {
                    return false;
                }

                // Monta o pacote
                var registro = new Documento
                {
                    Ativo = true,
                    NameOriginal = Dados.FileName,
                    NameArchived = NomeGerado,
                    Extension = Dados.extension,
                    DateAccessed = DateTime.Now,
                    DateCreated = DateTime.Now
                };

                // Adiciona os dados ao banco de dados
                _db.StorageRegistry.AddAsync(registro);

                //salva as alterações
                await _db.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }

        }

        public async Task<DocumentoDownloadDTO> ObterRegistro(string token)
        {

           var dados = await _db.StorageRegistry.FirstOrDefaultAsync(d => d.NameArchived == token);

            if(dados == null)
            {
               return null;   
            }    

            if(dados.Ativo == false)
            {
                return null;
            }

            var registro = new DocumentoDownloadDTO
            {
                NameArchived = dados.NameArchived,
                NameOriginal = dados.NameOriginal,
                Extension = dados.Extension,
                Ativo = dados.Ativo
            };

            return registro;            
        }

        public async Task<bool> UpdateDateAccessed(string NameArchived)
        {
            try
            {
                var dados = await _db.StorageRegistry.FirstOrDefaultAsync(d => d.NameArchived == NameArchived);

                if (dados == null || dados.Ativo == false)
                {
                    return false;
                }

                dados.DateAccessed = DateTime.Now;
                dados.QtdAcess = dados.QtdAcess + 1;

                await _db.SaveChangesAsync();
                return true;
            }
            catch
            { 
                return false; 
            }
        }

        public async Task<bool> Active(string NameArchived, bool valor)
        {
            try
            {
                var dados = await _db.StorageRegistry.FirstOrDefaultAsync(d => d.NameArchived == NameArchived);

                if (dados == null)
                {
                    return false;
                }

                dados.Ativo = valor;

                await _db.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Delete(string NameArchived)
        {
            try
            {
                var dados = await _db.StorageRegistry.FirstOrDefaultAsync(d => d.NameArchived == NameArchived);

                if (dados == null)
                {
                    return false;
                }

                _db.StorageRegistry.Remove(dados);

                await _db.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
