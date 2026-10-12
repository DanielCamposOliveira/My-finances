namespace Cloud_API.src.Background.BackDataScheduler
{
    public class DeleteDocumentService
    {
        private readonly ILogger<DeleteDocumentService> _logger;
        private readonly DictionaryStorage _repo;
        private readonly string RepositoryBase;


        public DeleteDocumentService(IConfiguration configuration, DictionaryStorage repo, ILogger<DeleteDocumentService> logger)
        {
            _repo = repo;
            _logger = logger;            
            RepositoryBase = configuration.GetSection("LocalSettings:PathStrings:BasePathRepositoryBase").Value;            
        }


        public async Task<IResult> Delete(RegistroDelete registro, CancellationToken token)
        {
            try
            {
                // Respeita o token antes de iniciar uma operação pesada de I/O
                token.ThrowIfCancellationRequested();

                // Construir os caminhos completos dos arquivos
                string filePathBase = Path.Combine(RepositoryBase, registro.FileName + registro.extension);
              
                if (File.Exists(filePathBase))
                {
                    await Task.Run(() => File.Delete(filePathBase), token);
                }              

                // Remove do dicionario o registro
                _repo.DicionarioDeMemoria.TryRemove(registro.Id.ToString(), out _);

                return Results.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar os arquivos relacionados ao registro com ID {Id}", registro.Id);
                return Results.Problem($"Erro: {ex.Message}");
            }
        }

    }
}
