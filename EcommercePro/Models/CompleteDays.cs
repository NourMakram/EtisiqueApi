using static EcommercePro.Models.Constants;

namespace EtisiqueApi.Models
{
    public class CompleteDays
    {
        public int Id { get; set; }
        public DateOnly date { set; get; }

        public ServiceTypeEnum serviceType { set; get; }
    }
}
