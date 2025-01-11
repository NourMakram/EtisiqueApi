using EcommercePro.Models;
using EtisiqueApi.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EtisiqueApi.DTO
{
	public class ProjectDto
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "project Name is Required")]
		[Unique]
    	public string ProjectName { get; set; }

		public string? Description { get; set; }

		[Required(ErrorMessage = "Neighborhood  is Required")]
		public string Neighborhood { get; set; }
		[Required(ErrorMessage = "City  is Required")]
		public string City { get; set; }

		[Required(ErrorMessage = "Address  is Required")]
		public string Address { get; set; }
		public string? ImageUrl { get; set; }
		public IFormFile? FileUrl { get; set; }

		[Required(ErrorMessage = "Buiding Number is Required")]
		public int BuidingNo { get; set; }

		[Required(ErrorMessage = "Unit Number is Required")]
		public int UnitNo { get; set; }
		public string? CreatedById { get; set; }



		

	}
	public class ProjectResponse
	{
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string? Description { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string? ImageUrl { get; set; }
        public int BuidingNo { get; set; }
        public int UnitNo { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDate { get; set; }
        public string? UpdatedDate { get; set; }
        public string ProjectStatus { get; set; }   // 1 on ,  2  off

    }
	public class projectList()
	{
		public int id { set; get; }
		public string name { set; get; }
	}

}
