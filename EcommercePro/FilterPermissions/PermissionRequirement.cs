using Microsoft.AspNetCore.Authorization;

namespace EtisiqueApi.FilterPermissions
{
    public class PermissionRequirement:IAuthorizationRequirement
    {
        public string Permission { get; set; }
        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
