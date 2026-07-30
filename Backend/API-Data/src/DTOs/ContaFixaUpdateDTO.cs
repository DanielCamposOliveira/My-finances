namespace API_Data.src.DTOs
{
    public class ContaFixaUpdateDTO
    {
        public record ContaFixaUpdateStatusDTO
        (
            int Id_ContaFixa,
            bool Status

        );

        public record ContaFixaUpdateDsTO
        (
            int Id_ContaFixa,
            decimal ValorBase

        );
    }
}
