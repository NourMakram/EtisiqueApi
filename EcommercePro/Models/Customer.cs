using EcommercePro.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EtisiqueApi.Models
{
	public class Customer
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "TypeProject is Required")]
		public string TypeProject { get; set; } //مالك - مستأجر

		[Required(ErrorMessage = "City is Required")]
		public string City { set; get; }

		[Required(ErrorMessage = "Building is Required")]
		public string BulidingName { get; set; }

		[Required(ErrorMessage = "UnitNo is Required")]
		public int UnitNo { get; set; }

		public List<Location>? Locations { set; get; }

		[ForeignKey("ApplicationUser")]
		[Required(ErrorMessage ="userid is Required")]
		public string UserId { get; set; }

		public ApplicationUser? ApplicationUser { get; set; }

		public bool IsReceived { get; set; } = false;
		public string? ReceivedCode { get; set; }
		public DateTime? ReceivedDate { set; get; }
    }
}
