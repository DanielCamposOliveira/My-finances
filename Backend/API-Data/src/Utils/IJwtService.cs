using static API_Data.src.DTOs.JwtDTO;

namespace API_Data.src.Utils
{
    public interface IJwtService
    {
        string GenerateToken(RegisterJWT user);
    }
}
