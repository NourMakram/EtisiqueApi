using System.ComponentModel.DataAnnotations;

namespace EtisiqueApi.DTO
{
    public class RuleDto
    {
        public class RuleDtoFoAdd()
        {
            [Required(ErrorMessage ="Enter The Text of Rule")]
            public string Text { get; set; }
            [Required(ErrorMessage ="Enter the Projcts")]
            public List<int> Projects { get; set; }

        }
        public class RuleDtoForUpdate()
        {
            [Required(ErrorMessage = "Enter The Id of Rule")]
            public int TextId { get; set; }
            [Required(ErrorMessage = "Enter The Text of Rule")]
            public string Text { get; set; }

        }
        public class RuleForShow()
        {
            public int id { get; set; }
            public string Text { get; set; } 
            public string Project { set; get; }

        }
    }
}
