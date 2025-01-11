using EcommercePro.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EtisiqueApi.DTO
{
	public class ContractDto
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "StartDate  is Required")]
		public DateOnly StartDate { get; set; }
		[Required(ErrorMessage = "EndDate  is Required")]
		public DateOnly EndDate { get; set; }
		[Required(ErrorMessage = "TcvIncTax  is Required")]
		public decimal TcvIncTax { set; get; }
		public string? description { get; set; }
		public string? contractFile { get; set; }
		public int Status { get; set; } = 1; //[ تم التجديد (2) - (1)منتهى - (0)متوفر];

       // [Required(ErrorMessage = "Contract File  is Required")]
		public IFormFile? FormFileUrl { get; set; }

		[Required(ErrorMessage = "project Id is Required")]
		public int projectId { get; set; }
	}
	public class ContractResponse
	{
        public int Id { set; get; }
        public string FilePath { set; get; }
        public string SatrtDate { set; get; }
        public string EndDate { set; get; }
        public string stuats { set; get; }
        public string TotalContracValue { set; get; }
        public string Description { set; get; }
		public int ProjectId { set; get; }

	}
}
