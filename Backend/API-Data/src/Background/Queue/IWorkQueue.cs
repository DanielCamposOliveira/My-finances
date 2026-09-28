namespace API_Data.src.Background
{
    /// <summary>
    /// Define o contrato para uma fila de trabalho assíncrona em segundo plano.
    /// Permite o agendamento e o consumo de tarefas diferidas.
    /// </summary>
    public interface IWorkQueue
    {
        /// <summary>
        /// Adiciona uma tarefa à fila de execução.
        /// </summary>
        ValueTask EnfileirarAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém e remove a próxima tarefa disponível na fila.
        /// Aguarda de forma assíncrona se a fila estiver vazia.
        /// </summary>



        ValueTask<string> DesenfileirarAsync(CancellationToken cancellationToken);

    }
}