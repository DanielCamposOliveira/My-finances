using Cloud_API.src.DTO;
using Cloud_API.src.Model;
using Microsoft.EntityFrameworkCore;

namespace Cloud_API.src.Repository.DataBase
{
    public class DataBaseRepository : IDataBaseRepository
    {

        private readonly IServiceScopeFactory _scopeFactory;

        public DataBaseRepository( IServiceScopeFactory scopeFactory)
        {    
            _scopeFactory = scopeFactory;
        }

        public async Task<bool> RegistraArquivo(UploadChunkDTO Dados, string NomeGerado)
        {

            try
            {
                // Cria um escopo de DI isolado para a thread em background
                using var scope = _scopeFactory.CreateScope();

                // Resolve o DbContext dentro deste novo escopo
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                
                // verifica se existe algum arquivo com o mesmo nome, porem isso vai ser muito mas muito RARO
                if (await dbContext.StorageRegistry.AnyAsync(d => d.NameArchived == NomeGerado))
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
                dbContext.StorageRegistry.AddAsync(registro);

                //salva as alterações
                await dbContext.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }

        }

        public async Task<DocumentoDownloadDTO> ObterRegistro(string token)
        {

            // Cria um escopo de DI isolado para a thread em background
            using var scope = _scopeFactory.CreateScope();

            // Resolve o DbContext dentro deste novo escopo
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();


            var dados = await dbContext.StorageRegistry.FirstOrDefaultAsync(d => d.NameArchived == token);

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
                // Cria um escopo de DI isolado para a thread em background
                using var scope = _scopeFactory.CreateScope();

                // Resolve o DbContext dentro deste novo escopo
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();


                var dados = await dbContext.StorageRegistry.FirstOrDefaultAsync(d => d.NameArchived == NameArchived);

                if (dados == null || dados.Ativo == false)
                {
                    return false;
                }

                dados.DateAccessed = DateTime.Now;
                dados.QtdAcess = dados.QtdAcess + 1;

                await dbContext.SaveChangesAsync();
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
                // Cria um escopo de DI isolado para a thread em background
                using var scope = _scopeFactory.CreateScope();

                // Resolve o DbContext dentro deste novo escopo
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();


                var dados = await dbContext.StorageRegistry.FirstOrDefaultAsync(d => d.NameArchived == NameArchived);

                if (dados == null)
                {
                    return false;
                }

                dados.Ativo = valor;

                await dbContext.SaveChangesAsync();

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
                // Cria um escopo de DI isolado para a thread em background
                using var scope = _scopeFactory.CreateScope();

                // Resolve o DbContext dentro deste novo escopo
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();


                var dados = await dbContext.StorageRegistry.FirstOrDefaultAsync(d => d.NameArchived == NameArchived);

                if (dados == null)
                {
                    return false;
                }

                dbContext.StorageRegistry.Remove(dados);

                await dbContext.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
