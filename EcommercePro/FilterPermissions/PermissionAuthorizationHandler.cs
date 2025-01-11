using EcommercePro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace EtisiqueApi.FilterPermissions
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        public PermissionAuthorizationHandler(RoleManager<ApplicationRole> roleManager) { 
            _roleManager = roleManager;
        
        }
        protected  override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null || !(context.User.Identity.IsAuthenticated))
                 return;
            var CurrentUser = context.User.Claims.Where(C=>C.Type == ClaimTypes.Role);
            var Roles = CurrentUser;

            foreach (var role in Roles)
            {
                string Role = role.Value;

                if (Role != null)
                {
                    ApplicationRole Roledb = _roleManager.Roles.FirstOrDefault(R => R.IsDeleted == false && R.Name == Role);
                    var Permission = await _roleManager.GetClaimsAsync(Roledb);
                    var canAccess = Permission.Any(C => C.Type == ClaimConstants.Permission && C.Value == requirement.Permission);

                    if (canAccess)
                    {
                        context.Succeed(requirement);
                        return;

                    }

                }
            }
            return;
                
          

        }
    }
}
