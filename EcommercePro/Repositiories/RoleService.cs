using EcommercePro.DTO;
using EcommercePro.Models;
using EcommercePro.Repositiories.Interfaces;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcommercePro.Repositiories
{

	public class RoleService : IRoleService
	{
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly Context _context;
		private IAcountService _AcountService;
		public RoleService(RoleManager<ApplicationRole> roleManager , Context context, IAcountService AcountService)
		{
			_roleManager = roleManager;
			_context = context;
			_AcountService= AcountService;

		}
		public async Task<(bool Succeeded, string[] Errors)> CreateRoleAsync(ApplicationRole role, IEnumerable<string> claims)
		{
			if (claims == null)
				claims = new string[] { };

			string[] invalidClaims = claims.Where(c => ApplicationPermissions.GetPermissionByValue(c) == null).ToArray();
			if (invalidClaims.Any())
				return (false, new[] { "The following claim types are invalid: " + string.Join(", ", invalidClaims) });


			var result = await _roleManager.CreateAsync(role);
			if (!result.Succeeded)
				return (false, result.Errors.Select(e => e.Description).ToArray());


			role = await _roleManager.FindByNameAsync(role.Name);

			foreach (string claim in claims.Distinct())
			{
				result = await this._roleManager.AddClaimAsync(role, new Claim(ClaimConstants.Permission, ApplicationPermissions.GetPermissionByValue(claim)));

				if (!result.Succeeded)
				{
					await DeleteRoleAsync(role);
					return (false, result.Errors.Select(e => e.Description).ToArray());
				}
			}

			return (true, new string[] { });
		}
		public async Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(string RoleId)
		{
			var roledb =  _roleManager.Roles.FirstOrDefault(R=>R.Id==RoleId && R.IsDeleted==false);
			if (roledb != null)
			{
				var Result = await DeleteRoleAsync(roledb);

				return (Result.Succeeded, Result.Errors);
			}
			return (false, new string[] { "Not Found The Role" });
		}

		public async Task<(bool Succeeded, string[] Errors)> DeleteRoleAsync(ApplicationRole role)
		{
			//IdentityResult Result = await _roleManager.DeleteAsync(role);
			role.IsDeleted = true;
			var Result = await _roleManager.UpdateAsync(role);

			if (Result.Succeeded)
			{
				return (true, new string[] { });

			};
			return (false, Result.Errors.Select(M => M.Description).ToArray());

		}


		public async Task<(bool Succeeded, string[] Errors)> UpdateRoleAsync(string RoleId, ApplicationRole role, IEnumerable<string> claims)
		{
			if (claims != null)
			{
				string[] invalidClaims = claims.Where(c => ApplicationPermissions.GetPermissionByValue(c) == null).ToArray();
				if (invalidClaims.Any())
					return (false, new[] { "The following claim types are invalid: " + string.Join(", ", invalidClaims) });
			}

			var roledb =  _roleManager.Roles.FirstOrDefault(R => R.Id == RoleId && R.IsDeleted == false);


			if (roledb == null)
			{
				return (false, new string[] { "Role Not Found" });
			}

			roledb.Name=role.Name;
			roledb.Description=role.Description;
			roledb.CreatedDate = roledb.CreatedDate;
			roledb.UpdatedDate = role.UpdatedDate;

			var result = await _roleManager.UpdateAsync(roledb);
			if (!result.Succeeded)
				return (false, result.Errors.Select(e => e.Description).ToArray());


			if (claims != null)
			{
				var roleClaims = (await _roleManager.GetClaimsAsync(roledb)).Where(c => c.Type == ClaimConstants.Permission);
				var roleClaimValues = roleClaims.Select(c => c.Value).ToArray();

				var claimsToRemove = roleClaimValues.Except(claims).ToArray();
				var claimsToAdd = claims.Except(roleClaimValues).Distinct().ToArray();

				if (claimsToRemove.Any())
				{
					foreach (string claim in claimsToRemove)
					{
						result = await _roleManager.RemoveClaimAsync(roledb, roleClaims.Where(c => c.Value == claim).FirstOrDefault());
						if (!result.Succeeded)
							return (false, result.Errors.Select(e => e.Description).ToArray());
					}
				}

				if (claimsToAdd.Any())
				{
					foreach (string claim in claimsToAdd)
					{
						result = await _roleManager.AddClaimAsync(roledb, new Claim(ClaimConstants.Permission, ApplicationPermissions.GetPermissionByValue(claim)));
						if (!result.Succeeded)
							return (false, result.Errors.Select(e => e.Description).ToArray());
					}
				}
			}

			return (true, new string[] { });
		}

		public async Task<List<ApplicationRole>> GetRolesAsync(int page, int pageSize)
		{

			IQueryable<ApplicationRole> rolesQuery = _context.Roles
				.Where(R=>R.IsDeleted==false)
				.OrderBy(r => r.Name);

			if (page != -1)
				rolesQuery = rolesQuery.Skip((page - 1) * pageSize);

			if (pageSize != -1)
				rolesQuery = rolesQuery.Take(pageSize);

			var roles = await rolesQuery.ToListAsync();

			return roles;
		}

		public async Task<RoleDto> GetRoleAsync(string RoleId)
		{
			ApplicationRole Roledb = _roleManager.Roles.Include(R => R.Claims).
				FirstOrDefault(R => R.Id == RoleId && R.IsDeleted == false);
			if(Roledb != null)
			{
				return new RoleDto()
				{
					Id = Roledb.Id,
					Name = Roledb.Name,
					Description = Roledb.Description,
					Permissions = Roledb.Claims.Select(C => C.ClaimValue).ToList(),

				};
			}

			return null;

			//ApplicationRole Roledb = _roleManager.Roles.FirstOrDefault(R => R.Id == RoleId && R.IsDeleted == false);

			//IList<Claim> Permissions = await _roleManager.GetClaimsAsync(Roledb);
			//List<string> PermissionsValue = Permissions.Select(P => P.Value).ToList();

			//return new RoleDto()
			//{
			//	Id= Roledb.Id ,
			//	Name=Roledb.Name,
			//	Description=Roledb.Description,
			//	Permissions=PermissionsValue

			//};



		}

		public List<PermissionList> GetPermissions()
		{
			return ApplicationPermissions.AllPermissions.GroupBy(permission=>permission.GroupName).Select(p=>new PermissionList()
			{
				GroupName = p.Key ,
				Permissions = p.Select(permission=>new Permission()
				{
					name=permission.Name,	
					value=permission.Value,
					Checked=permission.Checked


				}).ToList()

			}).ToList();
		}

        public async Task<(bool Succeeded,List<string>permissions) >GetPermissionsByUserId(string userId)
        { 
			ApplicationUser userdb =await _AcountService.GetUserByIdAsync(userId);
			if(userdb == null){
				return (false,null);
			}

			var userRoles = await _AcountService.GetRolesToUser(userdb);
			if(userRoles.Count() == 0)
			{
                return (false, new List<string>() { "Roles Not Found" });

            }
			List<string> PermissionsList = new List<string>();
            foreach (var role in userRoles)
			{
                ApplicationRole Role = await _roleManager.FindByNameAsync(role);

                if (Role != null)
                {
                    IList<Claim> permissions = await _roleManager.GetClaimsAsync(Role);
                    var permissionsRole = permissions?.Select(p => p.Value).ToList();
					PermissionsList.AddRange(permissionsRole);
                }




            }
            return (true, PermissionsList);



        }
        public async Task<ApplicationRole> GetRoleById(string RoleId)
		{
			return _roleManager.Roles.FirstOrDefault(Role => Role.Id == RoleId && Role.IsDeleted == false);
		}
        public async Task<ApplicationRole> GetRoleByName(string RoleName)
		{
            return _roleManager.Roles.FirstOrDefault(Role => Role.Name == RoleName && Role.IsDeleted == false);

        }

    }

}
