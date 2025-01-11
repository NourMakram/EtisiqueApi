using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EtisiqueApi.Repositiories
{
    public class ContractService:GenaricService<Contract> , IContractService
    {
        private readonly DbSet<Contract> _Contract;
        private readonly IEmailService _emailService;
        private readonly IProjectService _projectService;
        public ContractService(Context context, IEmailService emailService, IProjectService projectService) : base(context)
        {
            _Contract = context.Set<Contract>();
            _emailService = emailService;
            _projectService = projectService;
        }

        public async Task<ContractResponse> GetByProjectId(int projectId)
        {
            Contract? contract = _Contract.AsNoTracking()
                .FirstOrDefault(Contract => Contract.projectId == projectId && Contract.IsDeleted == false);
            if(contract != null)
            {
              // await ChangeContractSatutsAsync(contract.Id);
              // await SendMessageAsync(contract.Id);


                ContractResponse contractResponse = new ContractResponse()
                {
                    Id = contract.Id,
                    FilePath = contract.contractFile,
                    SatrtDate = contract.StartDate.ToString("yyy-MM-dd"),
                    EndDate = contract.EndDate.ToString("yyy-MM-dd"),
                    Description = contract.description,
                    stuats = ContractSatuts(contract.Status),
                    TotalContracValue = contract.TcvIncTax.ToString()
                };
                return contractResponse;
            }
            return null;
        }

        public override async Task<Contract> GetByIdAsync(int id)
        {
            return await _Contract.AsNoTracking()
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);
        }
        public string ContractSatuts(int satuts)
        {
            if (satuts == 0)
                 return "منتهى";
            else if (satuts == 1)
                 return "متوفر";
            else
                return "تم التجديد";
        }
        public async Task<(bool Succeeded, string[] Errors)> ChangeContractSatutsAsync(int id)
        {
            Contract contract = await GetByIdAsync(id);

            var DayDate = DateOnly.FromDateTime(DateTime.Now);
            if (contract.EndDate == DayDate)
            {
                contract.Status = 1;
                var result = Update(contract);
                if (!result.Succeeded)
                {
                    return (false, result.Errors);
                }
                Project project = await _projectService.GetByIdAsync(contract.projectId);
                project.ProjectStatus = 2;
               var result2 = _projectService.Update(project);
                if (!result2.Succeeded)
                {
                    return (false, result2.Errors);
                }

                return (true, new string[] { });
            }
            return (true, new string[] { });

        }
        public async Task<(bool Succeeded, string[] Errors)> SendMessageAsync(int id )
        {
            Contract contract = await GetByIdAsync(id);
            var ContractEndMonth = contract.EndDate.Month;
            var BeforeMonth = ContractEndMonth - 1;
            var DayDate = DateOnly.FromDateTime(DateTime.Now);
            var CurrentMonth = DayDate.Month;
            if (BeforeMonth == CurrentMonth)
            {
                contract.Status = 1;
                var result = Update(contract);
                if (!result.Succeeded)
                {
                    return (false, result.Errors);

                }
                var Message = "Contract Is Ended";
                var Result = await _emailService.SendEmailAsync("", Message, "Contract Ended");
                if (!Result.Succeeded)
                {
                    return (false, new string[] { Result.MessageError });

                }
            }

            return (true, new string[] { });

        }

    }
}
