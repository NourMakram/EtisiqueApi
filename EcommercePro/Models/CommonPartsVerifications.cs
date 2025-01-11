using EcommercePro.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.Models
{
    public class CommonPartsVerifications
    {
        public int Id { get; set; }
        [ForeignKey("Approver")]
        public string ApproverID { get; set; }
        public ApplicationUser? Approver { get; set; }
        public bool? IsApproved { get; set; } = false;
        public decimal Fees { set; get; }

        public RequestsCommonParts? Request { get; set; }
        [ForeignKey("Request")]
        public int RequestId { get; set; }
        public string? Note { get; set; }
        public string? File { set; get; }
        public string? TimeElapsed { set; get; }
    }
}
