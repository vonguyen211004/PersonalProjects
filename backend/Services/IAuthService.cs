using System.Threading.Tasks;
using backend.Models;
using backend.Models.DTOs;

namespace backend.Services
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(RegisterDto registerDto);
        Task<string> LoginAsync(LoginDto loginDto);
        Task<bool> UserExistsAsync(string username);
    }
}