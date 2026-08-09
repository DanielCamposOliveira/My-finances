using API_Data.src.Model;

namespace API_Data.src.Utils
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
