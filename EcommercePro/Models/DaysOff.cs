using System.ComponentModel.DataAnnotations;

namespace EtisiqueApi.Models
{
    public class DaysOff
    {
            public int Id { get; set; }
            [DataType(DataType.Date)]
            public DateOnly From { get; set; }
            [DataType(DataType.Date)]
            public DateOnly To { get; set; }
        
    }
}
