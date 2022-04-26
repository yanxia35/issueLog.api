using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IssueLog.API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using IssueLog.API.Dtos;
using IssueLog.API.Models;

namespace IssueLog.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        public readonly PacsContext _context;
        private readonly IConfiguration _config;
        public AuthRepository(PacsContext context, IConfiguration config)
        {
            _config = config;
            _context = context;

        }
        public bool IsAdmin(HttpContext context, out string Username)
        {
            Username = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
            return context.User.Claims.FirstOrDefault(x => x.Type == "Roles").Value == "admin" ? true : false;
        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null)
                return null;
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            };
            return true;
        }

        public async Task<User> ResetPassword(string username)
        {
            byte[] passwordHash, passwordSalt;

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null) return null;
            CreatePasswordHash(user.Username, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _context.SaveChangesAsync();
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            };
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.Username == username))
                return true;
            return false;
        }

        public async Task<bool> ResetAllUserPassword()
        {
            var users = await _context.Users.ToListAsync();
            foreach (User user in users)
            {
                await ResetPassword(user.Username);
            }
            return true;
        }

        public async Task<User> ChangePassword(UserForChangePasswordDto userForChangePasswordDto)
        {
            byte[] passwordHash, passwordSalt;
            var user = await Login(userForChangePasswordDto.Username, userForChangePasswordDto.OldPassword);
            if (user == null)
            {
                return null;
            }
            CreatePasswordHash(userForChangePasswordDto.NewPassword, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<UserPrivilege> GetUserPrivilege(string userId)
        {
            var userPrivilege = await _context.UserPrivileges.FirstOrDefaultAsync(x => x.UserId == userId);
            return userPrivilege;
        }

        public async Task<int> InsertNewUsers()
        {
            var users = await _context.Users.ToListAsync();
            var employees = await _context.Employees.Where(x => users.FirstOrDefault(i => i.UserId == x.Id) == null).Where(e => e.IsFormer != true).OrderBy(x=>x.Id).ToListAsync();
            int counter = 0;
            foreach (Employee employee in employees)
            {
                var user = new User();
                user.UserId = employee.Id;
                user.Username = employee.ShortName;
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(user.Username, out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                _context.Users.Add(user);
                counter++;
            }
            await _context.SaveChangesAsync();

            var userPriviliges = await _context.UserPrivileges.ToListAsync();
            employees = await _context.Employees.Where(x => userPriviliges.FirstOrDefault(i => i.UserId == x.Id) == null).Where(e => e.IsFormer != true).OrderBy(x=>x.Id).ToListAsync();
            var groups = _config.GetSection("AdminGroups").Get<List<string>>();
            foreach (Employee employee in employees)
            {
                var userPrivilege = new UserPrivilege();
                userPrivilege.UserId = employee.Id;
                if(groups.Contains(employee.GroupId)){
                    userPrivilege.CanEditProjects=true;
                    userPrivilege.CanEditPartCatalog = true;
                }
                _context.UserPrivileges.Add(userPrivilege);
            }
            await _context.SaveChangesAsync();
            return counter;
        }
    }
}