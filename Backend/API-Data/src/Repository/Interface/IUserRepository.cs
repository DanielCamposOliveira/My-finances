using API_Data.src.Model;
using API_Data.src.DTOs.Result;
using static API_Data.src.DTOs.UserDtos;

namespace API_Data.src.Repository.Interface
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(string id);

        Task<OperationResult> RegisterUserAsync(RegisterRequest user);

        Task<User?> GetUserByEmailAsync(string email);

        Task<OperationResult> DeactivateUserAsync(string userId);

        Task<OperationResult> DeleteUserAsync(string userId);


        Task<ExportPagUserResponse> GetUserPageAsync(string userId, int page, int limit);
    }
}
