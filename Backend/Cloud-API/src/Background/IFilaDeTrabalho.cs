namespace Cloud_API.src.Background
{
    public interface IFilaDeTrabalho
    {
        // Agora aceita uma FUNÇÃO que recebe um token e retorna uma Task
        ValueTask EnfileirarTarefaAsync(Func<CancellationToken, Task> tarefa);
        ValueTask<Func<CancellationToken, Task>> DesenfileirarTarefaAsync(CancellationToken cancelamento);
    }
}
