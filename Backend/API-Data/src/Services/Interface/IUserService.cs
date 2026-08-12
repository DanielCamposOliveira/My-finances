using static API_Data.src.DTOs.UserDtos;


namespace API_Data.src.Services.Interface
{
    public interface IUserService
    {
        Task<IResult> RegisterUserAsync(RegisterRequest User);
        Task<IResult> AuthenticationUserAsync(LoginRequest req);
        Task<IResult> DeactivateUserAsync(string userId, string UserActiver);
        Task<IResult> DeleteUser(string userId, string UserDelete);
        Task<IResult> GetUserInfo(string userId);        
        Task<IResult> ObterPageUserAsync(string userId, int page, int limit);
    }
}
