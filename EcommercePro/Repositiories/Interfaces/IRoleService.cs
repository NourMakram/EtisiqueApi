using EcommercePro.DTO;
using EcommercePro.Models;
using Microsoft.AspNetCore.Identity;
using System.Security;

namespace EcommercePro.Repositiories.Interfaces
{

	public interface IRoleService
	{
		Task<(bool Succeeded, string[] Errors)> CreateRoleAsync(ApplicationRole role, IEnumerable<string> claims);
		Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(string RoleId);
		Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(ApplicationRole role);
		Task<(bool Succeeded, string[] Errors)> UpdateRoleAsync(string RoleId, ApplicationRole role, IEnumerable<string> claims);
		Task<ApplicationRole>GetRoleById(string RoleId);
        Task<ApplicationRole> GetRoleByName(string RoleName);

        Task<List<ApplicationRole>> GetRolesAsync(int page, int pageSize);
		Task<RoleDto> GetRoleAsync(string RoleId);
		List<PermissionList> GetPermissions();
		public Task<(bool Succeeded, List<string> permissions)> GetPermissionsByUserId(string userId);

    }
}
