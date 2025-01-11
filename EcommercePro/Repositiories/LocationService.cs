using EcommercePro.Models;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EtisiqueApi.Repositiories
{
    public class LocationService : GenaricService<Location>, ILocationService
    {
        private readonly DbSet<Location> _Location;
        private readonly Context _context;
        public LocationService(Context context) : base(context)
        {
            _Location = context.Set<Location>();
            _context = context;
        }
        public Location GetLocationByCustomerId(int customerId , double Longitude, double Latitude)
        {
          return  _Location.AsNoTracking()
          .FirstOrDefault(L=>L.customerId == customerId && L.Longitude== Longitude && L.Latitude==Latitude);
        }
        public Location GetLocationByCustomerId(int customerId)
        {
            return _Location.AsNoTracking()
            .FirstOrDefault(L => L.customerId == customerId );
        }
        public async Task<(bool Succeeded, string[] Errors,int LocationId)> AddLocationAsync(int customerId, double Longitude, double Latitude)
        {
            Location CustomerLocation = GetLocationByCustomerId(customerId, Longitude, Latitude);
            if(CustomerLocation == null)
            {
                Location newlocation = new Location()
                {
                    customerId = customerId!=0 ? customerId : null ,
                    Longitude = Longitude ,
                    Latitude =Latitude

                };
              var Result = await  AddAsync(newlocation);
                if (!Result.Succeeded)
                {
                    return (false, Result.Errors,0);
                }
                return (true, new string[] { }, newlocation.Id);

            }

            return (true, new string[] { },CustomerLocation.Id);


        }

        public override async Task<Location> GetByIdAsync(int id)
        {
            return await _Location.AsNoTracking().FirstOrDefaultAsync(L=>L.Id == id );
        }

        public (bool Succeeded, string[] Errors ) UpdateLocation(int customerId, double Longitude, double Latitude)
        {
            Location CustomerLocation = GetLocationByCustomerId(customerId, Longitude, Latitude);
            if (CustomerLocation != null)
            {
                CustomerLocation.Longitude = Longitude;
                CustomerLocation.Latitude = Latitude;
                var Result = Update(CustomerLocation);
                if (!Result.Succeeded)
                {
                    return (false, Result.Errors);
                }
            }

            return (true, new string[] { });
        }
    }
   
}
