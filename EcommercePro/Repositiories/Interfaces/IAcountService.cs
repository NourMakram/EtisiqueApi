using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Pagination;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EtisiqueApi.Repositiories.Interfaces
{
    public interface IAcountService
    {
        Task<(bool Succeeded, string[] Errors)> CreateUserAsync(ApplicationUser user);
        public Task<(bool Succeeded, string[] Errors)> AssignUserToRolesAsync(ApplicationUser user, List<string> RolesName);
        Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(string userId);
        Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(string userId, ApplicationUser user);
        Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user);
        Task<(bool Succeeded, string[] Errors)> Disable(string userId , bool Disable);
        public Task<(bool Succeeded, string[] Errors)> RemoveUserFromRoles(ApplicationUser user, List<string> RoleNames);
        public  Task<(bool Succeeded, string[] Errors)> UpdateUserRolesAsync(ApplicationUser user, List<string> NewRoles);
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<bool> GetUserByEmailAsync(string email);
         Task<ApplicationUser> GetUserByEmailAsync2(string Email);
        JwtSecurityToken GenerateToken(List<Claim> claims);
        Task< List<Claim>> SetClaimsToUserAsync(ApplicationUser user);
        ApplicationUser GetUserByPhoneNumberAsync(string PhoneNumber);
        Task<(bool Succeeded, string[] Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<(bool Succeeded, string[] Errors)> UpdateCodeAsync(ApplicationUser user, string Code);
        Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(ApplicationUser user, string NewPassword);
        Task<(bool Succeeded, string[] Errors , List<UserBasicInfoDto> Users)> GetUsersByRole(string Role);
        IQueryable<userData> GetUsers(string search, string Role);
        Task<PaginatedResult<userData>> GetUsersRole(PaginatedResult<userData> users);
        public Task<List<string>> GetRolesToUser(ApplicationUser user);

        public Task<(bool Succeeded, string[] Errors)> ResetPasswordWay2(ApplicationUser user, string NewPassword);
        public Task<(bool Succeeded, string[] Errors, int id)> AddNewClient(RegisterDto registerDto);

        public List<int> GetUserProjects(string UserId);
        public List<Project> GetUserProjectsList(string UserId);

        public (bool Succeeded, string[] Errors)SetUserProjects(string UserId, List<int> Projects);

        public (bool Succeeded, string[] Errors) UpadteUserProjects(string UserId, List<int> Projects);

        //public List<UserProject> getProjects();
        public List<UserBasicInfoDto> GetEmployeesByProject(int projectId);

    }
}
