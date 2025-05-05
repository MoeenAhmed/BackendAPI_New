using Microsoft.Graph.Models;

namespace BackendAPI.Services
{
    public interface IUserService
    {
        Task<User?> GetUserInfo(string authCode);
    }
}
