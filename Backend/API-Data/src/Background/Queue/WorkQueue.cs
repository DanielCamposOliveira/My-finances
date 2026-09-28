using System.Threading.Channels;

namespace API_Data.src.Background
{
    /// <summary>
    /// Implementação thread-safe da fila de trabalho utilizando <see cref="Channel{T}"/>.
    /// </summary>
    public class WorkQueue : IWorkQueue
    {
        // Canal de comunicação assíncrono e não limitado para armazenar as tarefas pendentes
        private readonly Channel<string> _fila;

        public WorkQueue()
        {
            // Cria um canal sem limite fixo de capacidade
            _fila = Channel.CreateUnbounded<string>();
        }

        //// Escreve uma nova tarefa no canal assíncrono.    
        //public async ValueTask EnfileirarAsync( Func<CancellationToken, ValueTask> trabalho, CancellationToken cancellationToken = default)
        //{
        //    // Adiciona o item ao escritor do canal
        //    await _fila.Writer.WriteAsync(trabalho, cancellationToken);
        //}


        //// Lê a próxima tarefa do leitor do canal de forma assíncrona.   
        //public async ValueTask<Func<CancellationToken, ValueTask>> DesenfileirarAsync( CancellationToken cancellationToken)
        //{
        //    // Aguarda e consome o próximo item disponível no canal
        //    return await _fila.Reader.ReadAsync(cancellationToken);
        //}

        public async ValueTask EnfileirarAsync(string userId, CancellationToken cancellationToken = default)
        {
            await _fila.Writer.WriteAsync(userId, cancellationToken);
        }

        public async ValueTask<string> DesenfileirarAsync(CancellationToken cancellationToken)
        {
            return await _fila.Reader.ReadAsync(cancellationToken);
        }
    }
}