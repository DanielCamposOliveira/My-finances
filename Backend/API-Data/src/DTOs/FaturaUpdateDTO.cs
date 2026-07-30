using API_Data.src.Enum;

namespace API_Data.src.DTOs
{

public class FaturaUpdateDTO()
{
        public record FaturaUpdateStatusDTO
        (
            int ParcelaId,
            StatusParcela Status

        );

        public record FaturaUpdateDsTO
        (
            int ParcelaId,
            decimal ValorParcela

        );
    }

}
