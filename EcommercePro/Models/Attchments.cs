using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.Models
{
    public class Attchments
    {
        public int Id { get; set; }

        [ForeignKey("RequestManagement")]
        public int RequestManagementId { get; set; }
        public RequestManagement? RequestManagement { get; set; }

        public string imagePath { get; set; }
    }
}
