using EcommercePro.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EtisiqueApi.DTO
{
    public class ComplaintDto
    {
        public class ComplaintDtoForShow
        {
            public int Id { get; set; }
            public string Code {  get; set; }
             public int UnitNo { get; set; }
             public string BuildingCode { get; set; }

             public string Date { get; set; }  
            public string Stuats { get; set; }

             public string IssueDetails { get; set; }

             public string ProjectName { get; set; }

             public string ClientName { get; set; }
            public string ClientPhone { get; set; }

            public string TechincalName { get; set; }
            public string TechincalPhone { get; set; }

            public string ComplaintIssueFile { get; set; }
            public string CreatedBy { get; set; }
         }
        public class ComplaintDtoForAdd
        {
            public int Id { get; set; }
            [Required(ErrorMessage = "Enter UnitNo")]
            public int UnitNo { get; set; }
            [Required(ErrorMessage = "Enter BuildingCode")]
            public string BuildingCode { get; set; }

            [DataType(DataType.Date)]
            public DateTime Date { get; set; } = DateTime.UtcNow;

            [Required(ErrorMessage = "Enter IssueDetails")]
            public string IssueDetails { get; set; }

            [Required(ErrorMessage = "Enter ProjectId")]
            public int ProjectId { get; set; }

            [Required(ErrorMessage = "Enter ClientId")]
            public string ClientId { get; set; }
            [Required(ErrorMessage = "Enter CreatedById")]
            public string CreatedById {  get; set; }
            public string? ComplaintIssueFile { get; set; }
            public IFormFile? formFile { get; set; }
        }
    }
}
