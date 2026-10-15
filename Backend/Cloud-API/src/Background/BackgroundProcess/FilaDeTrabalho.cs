using System.Threading.Channels;

namespace Cloud_API.src.Background.BackgroundProcess
{
    public class FilaDeTrabalho : IFilaDeTrabalho
    {
        // O Channel é o "túnel". <string> é o tipo de dado que vamos passar.
        //  private readonly Channel<string> _fila;

        private readonly Channel<Func<CancellationToken, Task>> _canal;

        public FilaDeTrabalho()
        {
            // Criamos uma fila sem limite de tamanho (Unbounded)
            _canal = Channel.CreateUnbounded<Func<CancellationToken, Task>>();
        }

        // Este método é chamado para colocar uma nova mensagem na fila.
        public async ValueTask EnfileirarTarefaAsync(Func<CancellationToken, Task> tarefa)
        {
            // Escreve a mensagem dentro do túnel (Channel)
            await _canal.Writer.WriteAsync(tarefa);
        }

        // Este método é chamado para pegar a próxima mensagem da fila.
        public async ValueTask<Func<CancellationToken, Task>> DesenfileirarTarefaAsync(CancellationToken cancelamento)
        {
            // Aqui acontece o Bloqueio Assíncrono: 
            // Se não houver nada no canal, o código "pausa" aqui sem travar a thread.
            return await _canal.Reader.ReadAsync(cancelamento);
        }
    }
}
