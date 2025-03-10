using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EtisiqueApi.Repositiories
{
    public class CustomerService:GenaricService<Customer>,ICustomerService
    {
        private readonly DbSet<Customer> _Customer;
        private MessageSender2.IMessageSender _MessageService;
         public CustomerService(Context context, MessageSender2.IMessageSender MessageService) : base(context)
        {
            _Customer = context.Set<Customer>();
            _MessageService = MessageService;
         }
      
        public async Task<(bool Succeeded, string[] Errors) >Update(string userId,RegisterDto Entirty)
        {
            try
            {
                Customer customer = GetCustomerByUserID(userId);

                customer.BulidingName = Entirty.BulidingName;
                customer.City = Entirty.City;
                customer.UnitNo = Entirty.UnitNo;
                customer.TypeProject = Entirty.TypeProject;
                customer.UserId = userId;
                customer.ApplicationUser.projectId = Entirty.projectId;
                customer.ApplicationUser.PhoneNumber = Entirty.PhoneNumber;
                customer.ApplicationUser.Email = Entirty.Email;
                customer.ApplicationUser.UserName = Entirty.PhoneNumber;
                customer.ApplicationUser.FullName = Entirty.FullName;
                customer.ApplicationUser.Image =
                Entirty.Image != null ? Entirty.Image : customer.ApplicationUser.Image;

                var result = base.Update(customer);
                if (!result.Succeeded)
                {
                    return (false, result.Errors);
                }
                return (true, new string[] { });

            }
            catch (Exception ex)
            {
                return (true, new string[] {ex.Message });

            }

        }

        public Customer GetCustomerByUserID(string userId)
        {
            return _Customer
                .Include(C=>C.ApplicationUser)
                .FirstOrDefault(C => C.UserId == userId&& C.ApplicationUser.IsDeleted==false&& C.ApplicationUser.IsEnable==true);
        }
        public override async Task<Customer> GetByIdAsync(int id)
        {
            return await _Customer.AsNoTracking().Include(c => c.ApplicationUser).Include(c=>c.Project).Include(c=>c.Locations).
                FirstOrDefaultAsync(c => c.Id == id && c.ApplicationUser.IsDeleted == false && c.ApplicationUser.IsEnable == true);
        }

        public async Task<(bool Succeeded, string[] Errors)> SendRecervationCode(int id)
        {
            var trans =await BeginTransactionAsync();
            try
            {
                //genetate Code
                Random generator = new Random();
                string Code = generator.Next(0, 10000).ToString("D4");
                Customer customerdb = await GetByIdAsync(id);
                if (customerdb == null)
                {
                    return (false, new string[] { "Customer Not Found" });

                }
                //save code
                customerdb.ReceivedCode = Code;
                var result = base.Update(customerdb);
                if (!result.Succeeded)
                {
                    return (false, result.Errors);
                }
                //send message to user 
                string Message = MessageSender2.Messages.ReceiveMsg(customerdb.ApplicationUser.FullName, Code,customerdb.Project.ProjectName,customerdb.BulidingName,customerdb.UnitNo.ToString());
                var resultMsg = await _MessageService.Send3Async(customerdb.ApplicationUser.PhoneNumber, Message, null);
                if (!resultMsg)
                {
                    return (false, new string[] { });
                }
                Commit(trans);
                return (true, null);

            }
            catch (Exception ex)
            {
                Rollback(trans);
                return (false, new string[] {ex.Message });

            }


        }
        public IQueryable<Client> ClientsReservation(string projectName = null, string buildingName = null, string ClientName = null, string ClientPhone = null, DateOnly from = default, DateOnly to = default)
        {
            var results = _Customer.Include(C => C.ApplicationUser)
				.Include(C => C.Project)
				.OrderByDescending(C=>C.Id)
                .Where(c => c.IsReceived == true);

            if (projectName != null)
            {
                results = results.Where(c=>c.Project.ProjectName.Contains(projectName));
            }
            if (buildingName != null)
            {
                results = results.Where(c => c.BulidingName == buildingName);
            }
            if (ClientName != null)
            {
                results = results.Where(c => c.ApplicationUser.FullName.Contains(ClientName));
            }
            if (ClientPhone != null)
            {
                results = results.Where(c => c.ApplicationUser.PhoneNumber.Contains(ClientPhone));
            }
            if (from != default)
            {
                results = results.Where(c => DateOnly.FromDateTime((DateTime)c.ReceivedDate) >= from);

            }
            if (to != default)
            {
                results = results.Where(c => DateOnly.FromDateTime((DateTime)c.ReceivedDate) >= from && DateOnly.FromDateTime((DateTime)c.ReceivedDate) <= to);

            }
            return  results.
                Select(c => new Client()
                {
                    Id = c.Id,
                    FullName = c.ApplicationUser.FullName,
                    Email = c.ApplicationUser.Email,
                    phoneNumber = c.ApplicationUser.PhoneNumber,
                    UnitNum = c.UnitNo,
                    BuildingName = c.BulidingName,
                    project = c.Project.ProjectName,
                    ReceivedDate = c.ReceivedDate != null ? c.ReceivedDate.Value.ToString("dd-MM-yyyy hh:mm tt") : "No date available"


                }).AsQueryable();
        }
        public async Task<Client> GetUnitDetails(int UnitId)
        {

            Client client = await _Customer
                     .Where(c => c.Id==UnitId)
                     .Include(c => c.ApplicationUser)
                     .Include(a => a.Project)
                     .Select(c => new Client
                     {
                         Id = c.Id,
                         FullName = c.ApplicationUser.FullName,
                         Email = c.ApplicationUser.Email,
                         phoneNumber = c.ApplicationUser.PhoneNumber,
                         UnitNum = c.UnitNo,
                         BuildingName = c.BulidingName,
                         project = c.Project.ProjectName,
                         ReceivedCode = c.ReceivedCode,
                         ReceivedDate = c.ReceivedDate!= null ? c.ReceivedDate.Value.ToString("dd-MM-yyyy") : "No date available"
                     })
                     .FirstOrDefaultAsync();

            return client;
          }
        public bool IsHajarPRojct(int unitId)
        {
            var result = _Customer
                        .Include(c => c.ApplicationUser)
                        .Include(a => a.Project)
                        .Any(c => c.IsReceived == false && c.Project.ProjectName.Contains("هاجر") && c.Id == unitId);

         return result;
        }
        public async Task<(bool Succeeded, string[] Errors)> ConfirmReservation(int id, string Code)
        {
            Customer customerdb = await GetByIdAsync(id);
            if (customerdb == null)
            {
                return (false, new string[] { "Customer Not Found" });

            }
            //confirm Receivetion
            if(customerdb.ReceivedCode != Code)
            {
                return (false, new string[] { "Code Not Matched" });

            }
            var currentDate = DateTime.UtcNow;

            // Increase the hour by 3
            var dateAfter3Hours = currentDate.AddHours(3);
            customerdb.IsReceived = true;
            customerdb.ReceivedDate = dateAfter3Hours;
            var result = base.Update(customerdb);
            
            return (result.Succeeded, result.Errors);
            

        }

        public IQueryable<Client> UnitsManagement(int projectId,string buidingName, string ClientName,
            string ClientPhone,DateOnly from , DateOnly to)
        {
            var results = _Customer.Include(C => C.ApplicationUser)
                 .Include(C => C.Project)
                 .OrderByDescending(C => C.Id)
                 .Where(c =>  c.ApplicationUser.IsDeleted==false);
            if (projectId != 0)
            {
                results = results.Where(c => c.projectId == projectId);

            }

            if (buidingName != null)
            {
                results = results.Where(c => c.BulidingName == buidingName);
            }
            if (ClientName != null)
            {
                results = results.Where(c => c.ApplicationUser.FullName.Contains(ClientName));
            }
            if (ClientPhone != null)
            {
                results = results.Where(c => c.ApplicationUser.PhoneNumber.Contains(ClientPhone));
            }
            if (from != default)
            {
                results = results.Where(c => DateOnly.FromDateTime((DateTime)c.ReceivedDate) >= from);

            }
            if (to != default)
            {
                results = results.Where(c => DateOnly.FromDateTime((DateTime)c.ReceivedDate) >= from && DateOnly.FromDateTime((DateTime)c.ReceivedDate) <= to);

            }
            //if (reciveDate != default)
            //{
            //    results = results.Where(c => DateOnly.FromDateTime((DateTime)c.ReceivedDate) == reciveDate);

            //}
            return results.
                Select(c => new Client()
                {
                    Id=c.Id,
                    userId = c.UserId,
                    FullName = c.ApplicationUser.FullName,
                    Email = c.ApplicationUser.Email,
                    phoneNumber = c.ApplicationUser.PhoneNumber,
                    UnitNum = c.UnitNo,
                    BuildingName = c.BulidingName,
                    GuaranteeStart=c.GuaranteeStart,
                    GuaranteeEnd=c.GuaranteeEnd,
                    project = c.Project.ProjectName,
                    ReceivedDate = c.ReceivedDate != null ? c.ReceivedDate.Value.ToString("dd-MM-yyyy hh:mm tt") : "No date available"
                   

                }).AsQueryable();
        }

        public bool IsGuaranteeAvailable(string userId, int projectId, int unitNo, string buidingName)
        {
            DateOnly now = DateOnly.FromDateTime(DateTime.UtcNow);
           
           Customer unitData = _Customer
                       .Include(C => C.ApplicationUser)
                       .Include(C => C.Project)
                       .FirstOrDefault(R => R.UserId == userId && R.projectId == projectId && R.UnitNo == unitNo && R.BulidingName == buidingName);
            if (unitData == null)
            {
                return false;
            }
            if(unitData.GuaranteeEnd==null || unitData.GuaranteeEnd >= now)
            {
                return true;
            }
            
            return false;
        
        }

        public async Task<(bool Succeeded, string[] Errors)> UpdateUnitsManagement(UnitsManagmentDto unitsManagment)
        {
            try
            {
                Customer userUnit = await _Customer.FirstOrDefaultAsync(c => c.Id == unitsManagment.id);
                if (userUnit == null) {
                    return (false,new string[] { "Not found" });
                }
                userUnit.GuaranteeStart = unitsManagment.start;
                userUnit.GuaranteeEnd = unitsManagment.end;
                var result = base.Update(userUnit);

                return (result.Succeeded, result.Errors);

            }
            catch(Exception ex)
            {
                return (false, new string[] { ex.Message });

            }
        }
        public bool  delete(int Id)
        {
            var customer = _Customer.FirstOrDefault(c => c.Id == Id);
            if (customer == null)
            {
                return false;
            }
            customer.IsReceived = false;
            base.Update(customer);
            return true;
            

        }

		public List<unit> GetUnitsByUserId(string userId)
		{
          List<unit> units =  _Customer.Where(C => C.UserId == userId)
                .Include(C => C.Project)
                .Select(C => new unit()
                {
                    Id = C.Id,
                    UnitNum=C.UnitNo,
                    BuildingName=C.BulidingName,
                    project = C.Project.ProjectName

                }).ToList();
            return units;
		}

        public bool IsGuaranteeAvailable(int UnitId)
        {
            DateOnly now = DateOnly.FromDateTime(DateTime.UtcNow);

            Customer unitData = _Customer.FirstOrDefault(R => R.Id == UnitId);
            if (unitData == null)
            {
                return false;
            }
            if (unitData.GuaranteeEnd == null || unitData.GuaranteeEnd >= now)
            {
                return true;
            }

            return false;
        }

         

        public async Task<(bool Succeeded, string[] Errors)> AddUnit(NewUnit newUnit)
        {
            Customer Customerdb = GetCustomerByUserID(newUnit.UserId);
            if (Customerdb == null)
            {
                return (false,new string[]{ "customer Not Found" });
            }

            Customer Unit = new Customer()
            {
                IsReceived = false,
                BulidingName=newUnit.BuidingName,
                UnitNo=newUnit.UnitNo,
                projectId=newUnit.projectId,
                TypeProject=newUnit.TypeProject,
                UserId=newUnit.UserId,
                City=Customerdb.City,
                GuaranteeStart=newUnit.GuaranteeStart,
                GuaranteeEnd=newUnit.GuaranteeEnd
            };
           var result = await base.AddAsync(Unit);
            if (!result.Succeeded)
            {
                return (false, result.Errors);
            }
            return (true, null);
        }

        public async Task<(bool Succeeded, string[] Errors)> UnpdateUnit(NewUnit newUnit)
        {
            Customer Unitdb =await _Customer.FirstOrDefaultAsync(c => c.Id == newUnit.id && c.ApplicationUser.IsDeleted == false && c.ApplicationUser.IsEnable == true); 
            if (Unitdb == null)
            {
                return (false, new string[] { "Unit Not Found" });
            }

            Unitdb.BulidingName = newUnit.BuidingName;
            Unitdb.UnitNo = newUnit.UnitNo;
            Unitdb.projectId = newUnit.projectId;
            Unitdb.TypeProject = newUnit.TypeProject;
            Unitdb.GuaranteeStart = newUnit.GuaranteeStart;
            Unitdb.GuaranteeEnd = newUnit.GuaranteeEnd;
              var result =  base.Update(Unitdb);
            if (!result.Succeeded)
            {
                return (false, result.Errors);
            }
            return (true, null);
        }

        public List<unit> UnitsWithGuarantee(string userId)
        {
            DateOnly now = DateOnly.FromDateTime(DateTime.UtcNow);

          List<unit> Units  =  _Customer.Where(Uint => Uint.UserId == userId && 
          (Uint.GuaranteeEnd == null || Uint.GuaranteeEnd >= now)).
          Include(C => C.Project)
                .Select(C => new unit()
                {
                    Id = C.Id,
                    UnitNum = C.UnitNo,
                    BuildingName = C.BulidingName,
                    project = C.Project.ProjectName

                }).ToList();
            return Units;
        }
    }
}
