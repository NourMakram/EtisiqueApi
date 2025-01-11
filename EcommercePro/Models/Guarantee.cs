using EcommercePro.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static EcommercePro.Models.Constants;

namespace EtisiqueApi.Models
{
	public class Guarantee
	{
		public int Id { get; set; }

		//[Required(ErrorMessage = "Title  is Required")]
		//public string Title { get; set; }

		[Required(ErrorMessage = "StartDate  is Required")]
		public DateOnly StartDate { get; set; }

		[Required(ErrorMessage = "EndDate  is Required")]
		public DateOnly EndDate { get; set; }

		[Required(ErrorMessage = "Guarantee image  is Required")]
		public string FileUrl { get; set; }
        public int Status { get; set; } = 0; //[ تم التجديد (2) - (1)منتهى - (0)متوفر]

        public string? Notes { get; set; }
		public bool IsDeleted { get; set; } = false;

		[Required(ErrorMessage = "InsideApartment  is Required")]
		public bool InsideApartment { get; set; }

		[Required(ErrorMessage = "Guarantee type  is Required")]
		public int GuaranteeType  { get; set; }

		[Required(ErrorMessage = "project Id is Required")]
		[ForeignKey("Project")]
		public int projectId { get; set; }
		public  Project? Project { get; set; }
		
	}
}
