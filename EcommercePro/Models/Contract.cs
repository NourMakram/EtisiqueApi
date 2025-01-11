using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommercePro.Models
{
	public class Contract
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "StartDate  is Required")]
		public DateOnly StartDate { get; set; }
		[Required(ErrorMessage = "EndDate  is Required")]
		public DateOnly EndDate { get; set; }
		[Required(ErrorMessage = "TcvIncTax  is Required")]
		public decimal TcvIncTax { set; get; }
		public string? description { get; set; }
		[Required(ErrorMessage = "Contract File  is Required")]
		public string contractFile { get; set; }
		public int Status { get; set; } = 0; //[ تم التجديد (2) - (0)منتهى - (1)متوفر]

        [Required(ErrorMessage = "project Id is Required")]
		[ForeignKey("Project")]
		public int projectId { get; set; }
		public Project? Project { get; set; }
		public bool IsDeleted { get; set; } = false;



	}
}
