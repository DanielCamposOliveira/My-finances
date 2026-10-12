namespace Cloud_API.src.Utils
{
    public class SnowflakeGenerator
    {
        // Usamos uma data de início customizada (Epoch) para economizar espaço no número final.
        // Exemplo: 1 de Janeiro de 2024.
        private const long CustomEpoch = 1704067200000L;

        // Quantidade de bits para cada pedaço do ID
        private const int MachineIdBits = 5; // Permite até 32 máquinas diferentes (0 a 31)
        private const int SequenceBits = 12; // Permite gerar até 4096 IDs por milissegundo, por máquina

        // Cálculos de limite e deslocamento (Shift)
        private const long MaxMachineId = -1L ^ (-1L << MachineIdBits);
        private const long MachineIdShift = SequenceBits;
        private const long TimestampLeftShift = SequenceBits + MachineIdBits;
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);

        private readonly long _machineId; // ID da máquina que está gerando o ID
        private long _sequence = 0L; // Mantém o número de sequência para IDs gerados no mesmo milissegundo
        private long _lastTimestamp = -1L; // Mantém o último timestamp em que um ID foi gerado
        private readonly object _lock = new object(); // Trava de segurança para concorrência

        public SnowflakeGenerator()
        {
            _machineId = 1;
        }


        public long GerarProximoId()
        {
            lock (_lock)
            {
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                // AJUSTE 1: Tratamento inteligente do relógio voltando
                if (timestamp < _lastTimestamp)
                {
                    long offset = _lastTimestamp - timestamp;

                    // Se o relógio voltou 5ms ou menos, apenas aguardamos ele alinhar.
                    if (offset <= 5)
                    {
                        timestamp = EsperarProximoMilissegundo(_lastTimestamp);
                    }
                    else
                    {
                        // Só lança erro se o atraso for bizarro (ex: mudança manual de hora)
                        throw new Exception($"O relógio retrocedeu criticamente ({offset}ms). Risco de colisão.");
                    }
                }

                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & SequenceMask;

                    if (_sequence == 0)
                        timestamp = EsperarProximoMilissegundo(_lastTimestamp);
                }
                else
                {
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;

                return ((timestamp - CustomEpoch) << (int)TimestampLeftShift) |
                       (_machineId << (int)MachineIdShift) |
                       _sequence;
            }
        }


        // Espera até o próximo milissegundo se a sequência estourar
        private long EsperarProximoMilissegundo(long lastTimestamp)
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            while (timestamp <= lastTimestamp)
            {
                Thread.SpinWait(10); // Pequena espera para não sobrecarregar a CPU
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            return timestamp;
        }
    }
}
