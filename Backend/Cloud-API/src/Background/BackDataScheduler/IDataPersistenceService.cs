namespace Cloud_API.src.Background.BackDataScheduler
{
    public interface IDataPersistenceService
    {
        Task SincronizarBD();
    }
}
