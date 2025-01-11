namespace EtisiqueApi.Models
{
    public class ApartmentService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ApartmentServicesType>? ServicesTypes { get; set; }

    }
}
