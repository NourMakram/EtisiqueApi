using EcommercePro.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace EtisiqueApi.Models
{
    public class Rule
    {
        public int Id { get; set; }
        public string Text { get; set; }

        [ForeignKey("Project")]
        public int? ProjectId { get; set; }
        public Project? Project { get; set; }

        public DateTime LastUpdate { set; get; }  

        public int TextId { get; set; }

    }
}
