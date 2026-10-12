using Microsoft.EntityFrameworkCore;

namespace Cloud_API.src.Background.BackDataScheduler
{
    public class SQLiteDataPersistenceService : IDataPersistenceService
    {
        private readonly DictionaryStorage _dictionaryStorage;
        public SQLiteDataPersistenceService(DictionaryStorage dictionaryStorage)
        {
            _dictionaryStorage = dictionaryStorage;
        }

        // Sincroniza os dados da memória com o banco de dados, garantindo que o banco reflita exatamente o estado atual da memória
        public async Task SincronizarBD()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    //Carrega TODOS os registros do banco
                    var registrosBanco = await db.StorageRegistry.ToListAsync();

                    //Cria dicionário para lookup rápido
                    var dictBanco = registrosBanco.ToDictionary(x => x.TokenBase);

                    //Atualiza e insere a partir da memória
                    foreach (var item in _dictionaryStorage.DicionarioDeMemoria)
                    {
                        var token = item.Key;
                        var dadosMemoria = item.Value;

                        if (dictBanco.TryGetValue(token, out Registro? registroBanco))
                        {
                            registroBanco.TokenBase = dadosMemoria.TokenBase;

                            registroBanco.FileName = dadosMemoria.FileName;
                            registroBanco.DataUpload = dadosMemoria.DataUpload;
           
                        }
                        else
                        {
                            // INSERT 
                            await db.StorageRegistry.AddAsync(dadosMemoria);
                        }
                    }


                    //carrega os tokens da memória em um HashSet para comparação eficiente (normalizando para evitar problemas de espaços ou maiúsculas)
                    var memoriaTokensNormalizados = _dictionaryStorage.DicionarioDeMemoria.Keys
                        .Where(k => !string.IsNullOrEmpty(k))
                        .Select(k => k.Trim().ToLower())
                        .ToHashSet();

                    //Identifica os registros do banco que não estão na memória (ou têm token vazio) e marca para remoção
                    var tokensParaRemover = registrosBanco
                        .Where(x => string.IsNullOrEmpty(x.TokenBase)
                                    || !memoriaTokensNormalizados.Contains(x.TokenBase.Trim().ToLower()))
                        .ToList();

                    //Se houver registros para remover, remove do banco
                    if (tokensParaRemover.Any())
                    {
                        db.StorageRegistry.RemoveRange(tokensParaRemover);
                    }

                    await db.SaveChangesAsync();

                    Console.WriteLine("[SincronizarBD] Dados sincronizados com o banco com sucesso.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SincronizarBD] Erro: {ex.Message}");
            }
        }
    }
}
