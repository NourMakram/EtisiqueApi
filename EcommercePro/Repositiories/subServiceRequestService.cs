using EcommercePro.Models;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.EntityFrameworkCore;
using static EcommercePro.Models.Constants;

namespace EtisiqueApi.Repositiories
{
    public class subServiceRequestService : ISubServiceRequestService
    {
        private Context _dbContext;
        public subServiceRequestService(Context dbContext) { 

        _dbContext = dbContext;

        }
        public (bool Succeeded, string[] Errors) AddSubServices(int RequestId, List<int> subServices, int ServiceType)
        {
             
                try
                {
                    List<ApartmentSubServicesRequest> apartmentSubServices = new List<ApartmentSubServicesRequest>();
                    foreach (var service in subServices)
                    {
                        apartmentSubServices.Add(new ApartmentSubServicesRequest()
                        {
                            RequestId = RequestId,
                            ApServiceTypeId = service,
                            ServiceType = ServiceType

                        });

                    }
                    _dbContext.ApartmentSubServicesRequests.AddRange(apartmentSubServices);
                _dbContext.SaveChanges();
                    return (true, new string[] { });
                }
                catch (Exception ex)
                {
                    return (false, new string[] { ex.Message });

                }
            
        }

        public IQueryable<ApartmentService> GetServices()
        {
            return _dbContext.ApartmentServices.AsNoTracking().AsQueryable();
        }
        public IQueryable<ApartmentServicesType> GetApartmentServiceTypeByServiceId(int serviceId)
        {
            return _dbContext.ApartmentServicesTypes.AsNoTracking().Where(service => service.ApartmentServiceId == serviceId).AsQueryable();
        }

        public List<string> GetSubServices(int RequestId, int ServiceType)
        {
            var SubServices = _dbContext.ApartmentSubServicesRequests
                             .Include(s => s.ApartmentServicesType)
                             .Where(s => s.RequestId == RequestId && s.ServiceType == ServiceType)
                             .Select(S => S.ApartmentServicesType.Name).ToList();
            return SubServices;
        }
    }

}
