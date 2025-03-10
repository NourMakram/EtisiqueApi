using EcommercePro.Models;
using EtisiqueApi.DTO;

namespace EtisiqueApi.Repositiories.Interfaces
{
    public interface ILocationService:IGenaricService<Location>
    {
        public Task<(bool Succeeded, string[] Errors, int LocationId)> AddLocationAsync(int customerId,double Longitude, double Latitude);
        public (bool Succeeded, string[] Errors) UpdateLocation(int customerId, double Longitude, double Latitude);
        public Location GetLocationByCustomerId(int customerId, double Longitude, double Latitude);
        public Location GetLocationByCustomerId(int customerId);
        public Location GetLocationByUserId(string userId);

    }

}

