namespace Cloud_API.src.Service.Consulta
{
    public interface IConsultaService
    {
        public Task<IResult> ObterRegistro(string token);

        public Task<IResult> ObterTodosRegistro();
    }
}
