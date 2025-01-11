using System.ComponentModel.DataAnnotations.Schema;

namespace EcommercePro.Models
{
    public class UserProject
    {
        public int Id { get; set; }

        [ForeignKey("Project")]
        public int projectId { set; get; }
        public Project? Project { set; get; }

        [ForeignKey("ManageBy")]
        public string  ManageById { get; set; }
        public ApplicationUser? ManageBy { get; set; }

    }
}