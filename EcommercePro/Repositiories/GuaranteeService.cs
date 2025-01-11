using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace EtisiqueApi.Repositiories
{
    public class GuaranteeService : GenaricService<Guarantee>, IGuaranteeService
    {
        private readonly DbSet<Guarantee> _Guarantee;
        private readonly Context _context;
        private IApartmentServicesRequestService _ApertmentService;
       private ISubServiceRequestService _subServiceRequestService;

        public GuaranteeService(Context context, IApartmentServicesRequestService apartmentServicesRequest,
                    ISubServiceRequestService subServiceRequestService) : base(context)
        {
            _Guarantee = context.Set<Guarantee>();
            _context = context;
            _ApertmentService = apartmentServicesRequest;
            _subServiceRequestService = subServiceRequestService;
        }

        
        public async Task< List<GuaranteeResponse>> GetByProjectIdAsync(int projectId)
        {
            List<GuaranteeResponse> guarantee = _Guarantee.AsNoTracking().Where(Guarantee=> Guarantee.projectId == projectId && Guarantee.IsDeleted==false)
                .Select(guarantee=>new GuaranteeResponse()
                {
                    Id = guarantee.Id,
                    Notes = guarantee.Notes,
                    EndDate = guarantee.EndDate.ToString(),
                    StartDate=guarantee.StartDate.ToString(),
                    FilePath = guarantee.FileUrl,
                    Status = guarantee.Status==0?"منتهى" : "متوفر",
                    GuaranteeType =_subServiceRequestService.GetServices().FirstOrDefault(ser=>ser.Id == guarantee.GuaranteeType).Name,
                    InsideApartment = (guarantee.InsideApartment) ? "داخل الشقة" : "خارج الشقة",
                    projectId=guarantee.projectId
                }).ToList();
            
                return guarantee;
                
            
         }
        //public static string GuaranteeType(int guaranteeType , IApartmentServicesRequestService apartmentServicesRequest)
        //{
        //    string type = apartmentServicesRequest.GetServices().FirstOrDefault(type => type.Id == guaranteeType)?.Name;
        //    return type;
        //}
    //    public string GuaranteeType(int guaranteeType)
    //    {
    //        string GuaranteeType = _ApertmentService.GetServices().FirstOrDefault(type => type.Id == guaranteeType)?.Name;

    //    return GuaranteeType;
    //}
        public string GuaranteeStatus(int Status)
        {
            if (Status == 0)
                return "منتهى";
            else if (Status == 1)
                return "متوفر";
            else
                return "تم التجديد";

        }

        public async Task<(bool Succeeded , string [] Errors )> ChangeGuaranteeStatusAsync(int id)
        {
           Guarantee guarantee = await GetByIdAsync(id);
           
            var DayDate = DateOnly.FromDateTime(DateTime.Now); 
            if(guarantee.EndDate <= DayDate)
            {
                guarantee.Status = 1;
               var result = Update(guarantee);
                if (!result.Succeeded)
                {
                    return (false, result.Errors);
                }
                return (true, new string[] { });
            }
            return (true, new string[] { });

        }

        public override async Task<Guarantee> GetByIdAsync(int id)
        {

            return await _Guarantee.AsNoTracking().FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);
        }



    }
}
