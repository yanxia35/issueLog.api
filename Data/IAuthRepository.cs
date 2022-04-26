using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using IssueLog.API.Dtos;
using IssueLog.API.Models;

namespace IssueLog.API.Data {

    public interface IAuthRepository {
        Task<User> ResetPassword (string username);
        Task<User> ChangePassword (UserForChangePasswordDto userForChangePasswordDto);
        Task<User> Login (string username, string password);
        Task<bool> UserExists (string username);
        Task<bool> ResetAllUserPassword();
        bool IsAdmin (HttpContext context, out string Username);
        Task<UserPrivilege> GetUserPrivilege(string userId);
        Task<int> InsertNewUsers();

    }
}