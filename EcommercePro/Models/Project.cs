using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using EtisiqueApi.Models;

namespace EcommercePro.Models
{
	public class Project
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "project Name is Required")]
		public string ProjectName { get; set; }

		public string? Description { get; set; }

		[Required(ErrorMessage = "Neighborhood  is Required")]
		public string Neighborhood { get; set; }
		[Required(ErrorMessage = "City  is Required")]
		public string City { get; set; }

		[Required(ErrorMessage = "Address  is Required")]
		public string Address { get; set; }
		public string? ImageUrl { get; set; }

		[Required(ErrorMessage = "Buiding Number is Required")]
		public int BuidingNo { get; set; }

		[Required(ErrorMessage = "Unit Number is Required")]
		public int UnitNo { get; set; }

		[ForeignKey("CreatedBy")]
		public string? CreatedById { get; set; }
		public ApplicationUser? CreatedBy { get; set; }

		public DateTime? CreatedDate { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int ProjectStatus { get; set; } = 1;  // 1 on ,  2  off

		public bool IsDeleted { get; set; } = false;

		public List<Contract>? Contract { set; get; }
		public List<Guarantee>? Guarantee { set; get; }



	}
}