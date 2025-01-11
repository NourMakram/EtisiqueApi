using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommercePro.Models
{
	public class ApplicationRole : IdentityRole
	{
		public ApplicationRole(string roleName) : base(roleName)
		{

		}
		public ApplicationRole()
		{

		}
		public string? Description { get; set; }
		public bool IsDeleted { get; set; } = false;


		public DateOnly? CreatedDate { get; set; }
		public DateOnly? UpdatedDate { get; set; }

		public ICollection<IdentityUserRole<string>>? Users { get; set; }

		public ICollection<IdentityRoleClaim<string>>? Claims { get; set; }
	}
}
