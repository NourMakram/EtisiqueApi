using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Pagination;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stripe.Terminal;
using System.Drawing.Printing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EtisiqueApi.Repositiories
{
    public class AcountServiceL : IAcountService
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;
        private Context _context;
       private ICustomerService _customerService;
        private ILocationService _locationService;
        public AcountServiceL(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, Context context,
            ICustomerService customerService, ILocationService locationService)
        {

            _userManager = userManager;
            _roleManager = roleManager;
            _customerService = customerService;
            _locationService = locationService;
            _context = context;
        }
        public async Task<(bool Succeeded, string[] Errors)> AssignUserToRolesAsync(ApplicationUser user, List<string> RolesName)
        {
             
            var Result = await _userManager.AddToRolesAsync(user, RolesName);
            if (!Result.Succeeded)
            {
                return (false, Result.Errors.Select(Error => Error.Description).ToArray());

            }
            return (true, new string[] { });


        }
        public async Task<(bool Succeeded, string[] Errors)> CreateUserAsync(ApplicationUser userData)
        {
            bool EmailExists = await GetUserByEmailAsync(userData.Email);
            ApplicationUser userDb = _userManager.Users.FirstOrDefault(user => user.PhoneNumber == userData.PhoneNumber);
            if (EmailExists)
            {
                return (false, new string[] { "  الأيميل الألكترونى موجود بالفعل قم بتسجيل الدخول او ادخل ايميل اخر" });
            }
            if (userDb != null)
            {
                return (false, new string[] { " رقم الجوال موجود بالفعل قم بتسجيل الدخول او ادخل رقماَ اخر" });

            }
            IdentityResult Result = await _userManager.CreateAsync(userData, userData.PasswordHash);
            if (!Result.Succeeded)
            {
                return (false, Result.Errors.Select(e => e.Description).ToArray());
            }
            return (true, new string[] { });
        }
        public async Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(string userId)
        {
            ApplicationUser userdb = await GetUserByIdAsync(userId);
            if (userdb == null)
            {
                return (false, new string[] { "User Not Found" });
            }


            userdb.IsDeleted = true;
            var Result = await UpdateUserAsync(userdb);
            if (!Result.Succeeded)
            {
                return (false, Result.Errors);

            }
            return (true, new string[] { });


        }
        public async Task<(bool Succeeded, string[] Errors)> Disable(string userId, bool Disable)
        {
            ApplicationUser userdb = await GetUserByIdAsync(userId);
            if (userdb == null)
            {
                return (false, new string[] { "User Not Found" });
            }


            userdb.IsEnable = Disable;
            var Result = await UpdateUserAsync(userdb);
            if (!Result.Succeeded)
            {
                return (false, Result.Errors);

            }
            return (true, new string[] { });

        }
        public JwtSecurityToken GenerateToken(List<Claim> claims)
        {
            //key and algorithm
            var KeyStr = Encoding.UTF8.GetBytes("1s3r4e5g6h7j81s3r4e5g6h7j81s3r4e5g6h7j81s3r4e5g6h7j99");
            var Key = new SymmetricSecurityKey(KeyStr);
            SigningCredentials signingCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            
            //create Token
            JwtSecurityToken MyToken = new JwtSecurityToken(
               issuer: "http://localhost:5261",
               audience: "http://localhost:4200",
              // expires: DateTime.Now.AddMonths(5),
              expires: DateTime.Now.AddMinutes(5),
               claims: claims,
               signingCredentials: signingCredentials
               );
            return MyToken;

        }
        public async Task<List<Claim>> SetClaimsToUserAsync(ApplicationUser user)
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim("PhoneNumber", user.PhoneNumber));

            claims.Add(new Claim("Id", user.Id));

            var Roles = await _userManager.GetRolesAsync(user);
            foreach (var role in Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;

        }
        public async Task<bool> GetUserByEmailAsync(string email)
        {
            ApplicationUser userDb = await _userManager.FindByEmailAsync(email);
            if (userDb != null)
            {
                return true;
            }
            return false;
        }
        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _userManager.Users
                .FirstOrDefaultAsync(user => user.Id == userId && user.IsDeleted == false && user.IsEnable == true);
        }
        public async Task<ApplicationUser> GetUserByEmailAsync2(string Email)
        {
            return await _userManager.Users.FirstOrDefaultAsync(user => user.Email == Email && user.IsDeleted == false && user.IsEnable == true);

        }
        public ApplicationUser GetUserByPhoneNumberAsync(string PhoneNumber)
        {
            ApplicationUser userDb = _userManager.Users.FirstOrDefault(user => user.PhoneNumber == PhoneNumber
            && user.IsDeleted == false && user.IsEnable == true);
            if (userDb != null)
            {
                return userDb;
            }
            return null;

        }
        public IQueryable<userData>GetUsers(string search , string Role)
        {
            var query = from user in _userManager.Users
                        where user.IsDeleted == false
                        join userRole in _context.Set<IdentityUserRole<string>>()
                            on user.Id equals userRole.UserId
                        join roleEntity in _context.Roles
                            on userRole.RoleId equals roleEntity.Id
                        select new { User = user, Role = roleEntity };

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.User.FullName.Contains(search) || u.User.PhoneNumber.Contains(search));
            }

            if (!string.IsNullOrEmpty(Role))
            {
                query = query.Where(u => u.Role.Id == Role);
            }

            var result = query
                .GroupBy(u => new
                {
                    u.User.Id,
                    u.User.FullName,
                    u.User.Email,
                    u.User.PhoneNumber
                })
                .Select(g => new userData
                {
                    Id = g.Key.Id,
                    FullName = g.Key.FullName,
                    Email = g.Key.Email,
                    PhoneNumber = g.Key.PhoneNumber,
                    Role = g.Select(u => u.Role.Name).ToList()
                })
                .AsQueryable();

            return result;
             

             
             

        }
        public async Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(string userId, ApplicationUser user)
        {
            ApplicationUser userdb = await GetUserByIdAsync(userId);
            if (userdb == null)
            {
                return (false, new string[] { "User Not Found" });
            }

            userdb.PhoneNumber = user.PhoneNumber;
            userdb.FullName = user.FullName;
            userdb.Email = user.Email;
            userdb.Image = user.Image;
            userdb.UserName = user.Email;
            userdb.projectId = user.projectId;

 
            var Result = await UpdateUserAsync(userdb);

            if (!Result.Succeeded)
            {
                return (false, Result.Errors);

            }

            return (true, new string[] { });


        }
        public async Task<(bool Succeeded, string[] Errors)> RemoveUserFromRoles(ApplicationUser user, List<string> RoleNames)
        {

            var Result = await _userManager.RemoveFromRolesAsync(user, RoleNames);

            if (!Result.Succeeded)
            {
                return (false, Result.Errors.Select(Error => Error.Description).ToArray());

            }
            return (true, new string[] { });


        }
        public async Task<(bool Succeeded, string[] Errors)> UpdateUserRolesAsync(ApplicationUser user, List<string> NewRoles)
        {
            var Roles = await _userManager.GetRolesAsync(user);
            var AddedRoles = new List<string>();
            foreach(var NewRole in NewRoles)
            {
                if (Roles.IndexOf(NewRole) == -1)
                {
                    AddedRoles.Add(NewRole);
                }
            }
            var RemoveRoles = new List<string>();
            foreach (var CurrentRole in Roles)
            {
                if (NewRoles.IndexOf(CurrentRole) == -1)
                {
                    RemoveRoles.Add(CurrentRole);
                }
            }
            var R1 = await AssignUserToRolesAsync(user, AddedRoles);
                if (!R1.Succeeded)
                {
                    return (false, R1.Errors);
                }
                var R2 = await RemoveUserFromRoles(user, RemoveRoles);
                if (!R2.Succeeded)
                {
                    return (false, R2.Errors);
                }
            
            return (true, new string[] { });



        }
        public async Task<(bool Succeeded, string[] Errors)> UpdateUserAsync(ApplicationUser user)
        {
            var Resutl = await _userManager.UpdateAsync(user);
            if (!Resutl.Succeeded)
            {
                return (false, Resutl.Errors.Select(Error => Error.Description).ToArray());
            }

            return (true, Resutl.Errors.Select(Error => Error.Description).ToArray());

        }
        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            bool found = await this._userManager.CheckPasswordAsync(user, password);
            return found;

        }
        public async Task<(bool Succeeded, string[] Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            ApplicationUser userdb = await GetUserByIdAsync(userId);
            if (userdb == null)
            {
                return (false, new string[] { "User Not Found" });
            }

            var Result = await _userManager.ChangePasswordAsync(userdb, currentPassword, newPassword);
            if (!Result.Succeeded)
            {
                return (false, Result.Errors.Select(Error => Error.Description).ToArray());
            }
            return (true, new string[] { });
        }
        public async Task<(bool Succeeded, string[] Errors)> UpdateCodeAsync(ApplicationUser user, string Code)
        {
            user.Code = Code;
            var Result = await _userManager.UpdateAsync(user);
            return (Result.Succeeded, Result.Errors.Select(Error => Error.Description).ToArray());
        }
       public  async Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(ApplicationUser user, string NewPassword)
        {
            var Result = await _userManager.RemovePasswordAsync(user);
            if (!Result.Succeeded)
            {
                return (false, Result.Errors.Select(Error => Error.Description).ToArray());
            }
            var Result2 = await _userManager.AddPasswordAsync(user, NewPassword);

            return (Result2.Succeeded, Result2.Errors.Select(Error => Error.Description).ToArray());

        }
        public async Task<(bool Succeeded , string[] Errors)> ResetPasswordWay2(ApplicationUser user, string NewPassword)
        {
            string resetToken = await this._userManager.GeneratePasswordResetTokenAsync(user);

            IdentityResult result1 = await this._userManager.ResetPasswordAsync(user, resetToken, NewPassword);

            if (!result1.Succeeded)
            {
                return (false, result1.Errors.Select(Error => Error.Description).ToArray());
            }

            return (true , result1.Errors.Select(Error => Error.Description).ToArray());


        }

        public async Task<(bool Succeeded, string[] Errors,List<UserBasicInfoDto> Users)> GetUsersByRole(string Role)
        {
          ApplicationRole Roledb = await  _roleManager.FindByNameAsync(Role);
            if (Roledb == null)
            {
                return (false, new string[] { "Invaild Role Name" }, null);
            }
            var Users = await _userManager.GetUsersInRoleAsync(Role);
           List<UserBasicInfoDto> userBasicInfoDtos = Users.Where(user=>user.IsDeleted==false).Select(user => new UserBasicInfoDto()
            {
                Id = user.Id,
                Name = user.FullName
             }).ToList();

            return (true, null, userBasicInfoDtos);
        }
        public async Task<List<string>> GetRolesToUser(ApplicationUser user)
        {
            var Roles = await _userManager.GetRolesAsync(user);
             

            return Roles.ToList();
        }
        public async Task<PaginatedResult<userData>> GetUsersRole(PaginatedResult<userData> users)
        {

            foreach (var user in users.Data)
            {
               var userdb =  await GetUserByIdAsync(user.Id);
                var Roles = await _userManager.GetRolesAsync(userdb);
                user.Role = Roles.ToList();



            }
            return users;

            

        }

        public async Task<(bool Succeeded, string[] Errors, int id)> AddNewClient(RegisterDto registerDto)
        {
           
                 try
                {
                    ApplicationUser user = new ApplicationUser()
                    {
                        FullName = registerDto.FullName,
                        Email = registerDto.Email,
                        PhoneNumber = registerDto.PhoneNumber,
                        PasswordHash = registerDto.Password,
                        UserName = registerDto.PhoneNumber,
                        projectId = registerDto.projectId,
                        Image = registerDto.Image != null ? registerDto.Image : "https://etsaq.com/be/Users/user.png"

                    };
                    var Result1 = await CreateUserAsync(user);

                    if (!Result1.Succeeded)
                    {
                        return (false,Result1.Errors,0);
                    }

                    Customer customer = new Customer()
                    {
                        UserId = user.Id,
                        BulidingName = registerDto.BulidingName,
                        UnitNo = registerDto.UnitNo,
                        City = registerDto.City,
                        projectId=registerDto.projectId,
                        TypeProject = registerDto.TypeProject,
                    };
                    var Result3 = await _customerService.AddAsync(customer);
                    if (!Result3.Succeeded)
                    {
                    return (false, Result3.Errors,0);

                }

                var Result4 = await _locationService.AddLocationAsync(customer.Id, registerDto.Longitude, registerDto.Latitude);
                    if (!Result4.Succeeded)
                    {
                        return (false, Result4.Errors,0);
                    }


                    var Result5 = await AssignUserToRolesAsync(user, ["عميل"]);
                    if (!Result5.Succeeded)
                    {
                        return (false, Result5.Errors,0);
                    }


                   return (true,null, customer.Id);



            }
            catch (Exception ex)
                {
                     
                 return (false, new string[] {ex.Message},0);


            }

        }

        public List<int> GetUserProjects(string UserId)
        {
           return  _context.UserProjects.Include(p=>p.Project)
                .Where(p=>p.ManageById == UserId && p.Project.IsDeleted==false)
                .Select(p=>p.projectId).ToList();
        }
        public List<Project> GetUserProjectsList(string UserId)
        {
            return _context.UserProjects.Include(p => p.Project)
                 .Where(p => p.ManageById == UserId && p.Project.IsDeleted == false)
                 .Select(p => new Project() {
                         ProjectName=p.Project.ProjectName,
                         Id=p.projectId
                 }).ToList();
        }


        public (bool Succeeded, string[] Errors)SetUserProjects(string UserId, List<int> Projects)
        {
            try
            {
                List<UserProject> userProjects = new List<UserProject>();
                foreach (var project in Projects)
                {
                    userProjects.Add(new UserProject()
                    {
                        projectId = project,
                        ManageById = UserId,

                    });

                }
                _context.UserProjects.AddRange(userProjects);
                _context.SaveChanges();
                return (true,null);
            }
            catch(Exception ex)
            {
                return (false,new string[] { ex.Message });
            }
        }

        public (bool Succeeded, string[] Errors) UpadteUserProjects(string UserId, List<int> Projects)
        {
           
            try
            {
                IQueryable<UserProject> CurrentuserProjects = _context.UserProjects.Where(p => p.ManageById == UserId);
                
                if(Projects==null && CurrentuserProjects.IsNullOrEmpty())
                {
                    return (true, null);

                }
                else if(Projects==null && CurrentuserProjects.Count() > 0)
                {
                    _context.UserProjects.RemoveRange(CurrentuserProjects);
                    _context.SaveChanges();
                    return (true, null);

                }
                _context.UserProjects.RemoveRange(CurrentuserProjects);
 
                List<UserProject> userProjects = new List<UserProject>();
                foreach (var project in Projects)
                {
                    userProjects.Add(new UserProject()
                    {
                        projectId = project,
                        ManageById = UserId,

                    });

                }
                _context.UserProjects.AddRange(userProjects);
                _context.SaveChanges();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new string[] { ex.Message });
            }



        }
        

        public List<UserBasicInfoDto> GetEmployeesByProject(int projectId)
        {
          List<UserBasicInfoDto> users=  _context.UserProjects.Include(p=>p.ManageBy).Where(p => p.projectId == projectId)
                .Select(p=>new UserBasicInfoDto()
          {
              Id=p.ManageById,
              Name=p.ManageBy.FullName

          }).ToList();
            return users;
        }

        //public List<UserProject> getProjects()
        //{
        //    List<UserProject> projects= _context.UserProjects.ToList();
        //    _context.UserProjects.RemoveRange(projects);
        //    _context.SaveChanges();
        //    return null;
        //}

    }



}
