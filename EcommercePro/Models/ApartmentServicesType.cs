using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.Models
{
    public class ApartmentServicesType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("ApartmentServices")]
        public int ApartmentServiceId { get; set; }
        public ApartmentService? ApartmentServices { get; set; }



    }
}
