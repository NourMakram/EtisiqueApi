using EtisiqueApi.Models;

namespace EtisiqueApi.Repositiories.Interfaces
{
    public interface ISubServiceRequestService
    {
        public (bool Succeeded, string[] Errors) AddSubServices(int RequestId, List<int> subServices, int ServiceType);
        public IQueryable<ApartmentService> GetServices();
        public IQueryable<ApartmentServicesType> GetApartmentServiceTypeByServiceId(int serviceId);

        public List<string> GetSubServices(int RequestId, int ServiceType);
    }
}
