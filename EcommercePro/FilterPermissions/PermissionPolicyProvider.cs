using EcommercePro.DTO;
using EcommercePro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace EtisiqueApi.FilterPermissions
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider DefaultAuthorizationPolicyProvider  { get; set; }
        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) {

           DefaultAuthorizationPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        
        }
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return DefaultAuthorizationPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return DefaultAuthorizationPolicyProvider.GetDefaultPolicyAsync();

        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            ApplicationPermission applicationPermission = ApplicationPermissions.GetPermissionByValue(policyName);
            if (applicationPermission != null)
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(policyName));

                return Task.FromResult(policy.Build());

            }
            return DefaultAuthorizationPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}
