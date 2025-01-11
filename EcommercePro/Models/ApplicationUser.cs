using EntityFrameworkCore.EncryptColumn.Attribute;
using Microsoft.AspNetCore.Identity;
using Stripe.Terminal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static EcommercePro.Models.Constants;

namespace EcommercePro.Models
{
    public class ApplicationUser : IdentityUser
    {
			public string FullName { get; set; }
			public bool? IsEnable { set; get; } = true;

		    [EncryptColumn]
		    public string? Code { get; set; }
		    public bool? IsDeleted { set; get; } = false;
			public string? Image { set; get; }

			[NotMapped]
			public IFormFile? formFile { set; get; }

		   [ForeignKey("Project")]
		    public int? projectId { set; get; }
		    public Project? Project { set; get; }

		    public ICollection<UserProject>? UserProjects { get; set; }	


		
    }
}
