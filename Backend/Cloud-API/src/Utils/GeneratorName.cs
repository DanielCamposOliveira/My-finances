using HashidsNet;

namespace Cloud_API.src.Utils
{
    public class GeneratorName : IGeneratorName
    {
        SnowflakeGenerator gerador;
        private readonly Hashids hashids;


        public GeneratorName(IConfiguration configuration, SnowflakeGenerator _gerador)
        {
            gerador = _gerador;

            // Pega o salt do configuration e define o tamanho  7 caracteres
            string salt = configuration.GetSection("config:SaltSecret").Value ?? "salt-padrao";
            hashids = new Hashids(salt, 7);
        }


        public string CreateName()
        {
            // Gera o número Snowflake à prova de colisões
            long numericId = gerador.GerarProximoId();

            // Ofusca o número usando o Hashids (método correto é EncodeLong)
            string hashId = hashids.EncodeLong(numericId);

            return hashId;
        }



    }
}
