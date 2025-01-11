using EcommercePro.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace EtisiqueApi.Models
{
    public class TechnicianQusetions
    {
        public int Id { get; set; }
        public int questionId { get; set; }
        public int AnswerId { get; set; }

        [ForeignKey("Technician")]
        public string TechnicianId { get; set; }
        public ApplicationUser? Technician { set; get; }

        [ForeignKey("questionnaire")]
        public int questionnaireId { get; set; }
        public Questionnaire? questionnaire { get; set; }
        public DateOnly CreatedDate { set; get; }


    }
}
