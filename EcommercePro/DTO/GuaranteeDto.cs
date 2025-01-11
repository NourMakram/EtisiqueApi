using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EtisiqueApi.DTO
{
	public class GuaranteeDto
	{
		public int Id { get; set; }

        [Required(ErrorMessage = "StartDate  is Required")]
        public DateOnly StartDate { get; set; }

        //[Required(ErrorMessage = "Title  is Required")]
        //public string Title { get; set; }

        [Required(ErrorMessage = "EndDate  is Required")]
		public DateOnly EndDate { get; set; }

		public string? FileUrl { get; set; }

		//[Required(ErrorMessage = "Guarantee image  is Required")]
		public IFormFile? formFileUrl { get; set; }
        public int Status { get; set; } = 0; //[(1)منتهى - (0)متوفر]

        public string? Notes { get; set; }

		[Required(ErrorMessage = "InsideApartment  is Required")]
		public bool InsideApartment { get; set; } // InsideApartment = true , commonApart = false

		[Required(ErrorMessage = "Guarantee type  is Required")]
		public int GuaranteeType { get; set; }

		[Required(ErrorMessage = "project Id is Required")]
		public int projectId { get; set; }
	}
    public class GuaranteeResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public  string StartDate { get; set; }
        public string EndDate { get; set; }
        public string? FilePath { get; set; }
        public string Status { get; set; } //[(1)منتهى - (0)متوفر]
        public string? Notes { get; set; }
        public string InsideApartment { get; set; } // InsideApartment = true , commonApart = false
        public string? GuaranteeType { get; set; }
		public int projectId { set; get; }

    }
}
