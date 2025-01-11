using EcommercePro.Models;
using static EcommercePro.Models.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.Models
{
    public class Complaint
    {
        
        public int Id { get; set; }
        public int Number { get; set; }
        public int UnitNo { get; set; }
        public string BuildingCode { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string IssueDetails { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
        public string Status { get; set; }

        public ApplicationUser? TransferredTo { get; set; }
        public ApplicationUser? Client { get; set; }

        [ForeignKey("TransferredTo")]
        public string? TransferredToId { get; set; }
        [ForeignKey("Client")]
        public string ClientId { get; set; }
        public string?  ComplaintIssueFile { get; set; }

        [ForeignKey("CreatedBy")]
        public string  CreatedById  { get; set; }
        public ApplicationUser? CreatedBy { get; set; }

    }
}
