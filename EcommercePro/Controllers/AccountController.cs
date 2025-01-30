using EcommercePro.Models;
using EcommercePro.Repositiories.Interfaces;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Pagination;
using EtisiqueApi.Repositiories;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
 
namespace EcommercePro.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private IAcountService _AcountService;
        private IFileService _fileService;
        private ICustomerService _customerService;
        private IEmailService _emailService;
        private ILocationService _locationService;
        private IRoleService _roleService;
        private MessageSender2.IMessageSender _MessageSender;
        private IProjectService _ProjectService;
        public AccountController(IAcountService AcountService, ILocationService locationService,
            UserManager<ApplicationUser> userManager , IFileService fileService, ICustomerService customerService,
            IEmailService emailService,IProjectService projectService  , IRoleService roleService,MessageSender2.IMessageSender messageSender
            )
		{

			_AcountService = AcountService;
            _fileService=fileService;
            _customerService = customerService;
            _emailService = emailService;
            _locationService = locationService;
            _roleService = roleService;
            _MessageSender = messageSender;
            _ProjectService = projectService;
		}
		[HttpPost("Create/User")]
        [Authorize(policy: "users.manage")]
        public async Task<IActionResult> CreateUser([FromBody]UserDto userDto)
		{
            if (ModelState.IsValid)
            {
                var trans = await _customerService.BeginTransactionAsync();
                try
                {
                    //genetate Code
                    Random generator = new Random();
                    string Code = generator.Next(0, 100000).ToString("D6");
                    var currentDate = DateTime.UtcNow;
                    // Increase the hour by 3
                    var dateAfter3Hours = currentDate.AddHours(3);
                    if (userDto.formFile != null)
                    {
                      var Result = await _fileService.SaveImage("Users",userDto.formFile);
                        if (!Result.Succeeded)
                        {
                            return BadRequest(Result.Errors);
                        }
                        userDto.Image = Result.imagePath;
                    }

                    ApplicationUser user = new ApplicationUser()
                    {
                        FullName = userDto.FullName,
                        Email = userDto.Email,
                        PhoneNumber = userDto.PhoneNumber,
                        PasswordHash = userDto.Password,
                        UserName = userDto.PhoneNumber,
                        projectId = userDto.projectId==0? null: userDto.projectId,
                        Image=userDto.Image!=null?userDto.Image : "https://etsaq.com/be/Users/user.png",
                        Code = Code


                    };
                    var result = await _AcountService.CreateUserAsync(user);
                    if (!result.Succeeded)
                    {
                        return BadRequest(result.Errors);
                    }
                    bool IsClient = userDto.Job.Contains("عميل");
                    if(IsClient&&userDto.Longitude!=null && userDto.Longitude != null)
                    {
                        
                        Customer customer = new Customer()
                        {
                            TypeProject = userDto.TypeProject,
                            BulidingName = userDto.BulidingName,
                            UnitNo = (int)userDto.UnitNo,
                            City = userDto.City,
                            UserId = user.Id,
                            projectId=userDto.projectId,
                            ReceivedDate=currentDate
                        };
                        var Result3 = await _customerService.AddAsync(customer);
                        if (!Result3.Succeeded)
                        {
                            return BadRequest(Result3.Errors);
                        }

                        var Result4 = await _locationService.AddLocationAsync(customer.Id, (double)userDto.Longitude, (double)userDto.Latitude);
                        if (!Result4.Succeeded)
                        {
                            return BadRequest(Result4.Errors);
                        }
                    }

                    
                    var resultData = await _AcountService.AssignUserToRolesAsync(user, userDto.Job);
                    if (!resultData.Succeeded)
                    {
                        return BadRequest(resultData.Errors);
                    }

                    if(userDto.Projects.Count() > 0)
                    {
                        var result5 = _AcountService.SetUserProjects(user.Id, userDto.Projects);
                        if (!result5.Succeeded)
                        {
                            return BadRequest(result5.Errors);


                        }
                    }
                    
                    var Message = MessageSender2.Messages.ConfrimEmail(Code);
                    var Result1 = await _MessageSender.Send3Async(user.PhoneNumber, Message, null);
                    if (!Result1)
                    {
                        return BadRequest("Faild To Confirm Email Try again");
                    }

                    var Result6 = await _emailService.SendEmailAsync(user.Email, Message, "Confirm Email");
                    if (!Result6.Succeeded)
                    {
                        return BadRequest("Faild To Confirm Email Try again");
                    }
                    _customerService.Commit(trans);

                    return Ok();
                }
                catch (Exception ex)
                {
                    _customerService.Rollback(trans);
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest(ModelState);

        }

		[HttpPut("Update/User/{userId}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(string userId ,updateDto userDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser userdb = await _AcountService.GetUserByIdAsync(userId);
                    ApplicationUser EmailUnique = await _AcountService.GetUserByEmailAsync2(userId);


                    if (userdb == null)
                    {
                        return BadRequest("User Not Found");
                    }

                    if (userDto.formFile != null)
                    {
                        var Result = await _fileService.ChangeImage("Users", userDto.formFile, userdb.Image);
                        if (!Result.Succeeded)
                        {
                            return BadRequest(Result.Errors);
                        }
                        userDto.Image = Result.imagePath;
                    }


                    userdb.FullName = userDto.FullName;
                    userdb.Email = userDto.Email;
                    userdb.PhoneNumber = userDto.PhoneNumber;
                    userdb.UserName = userDto.Email;
                    userdb.Image = userDto.Image;
                    userdb.projectId=userDto.projectId!=0 ? userDto.projectId:null;


                    if (userDto.Password != null)
                    {
                        var ResetPasswordResult = await _AcountService.ResetPasswordAsync(userdb, userDto.Password);
                        if (!ResetPasswordResult.Succeeded)
                        {
                            return BadRequest(ResetPasswordResult.Errors);

                        }
                    }

                    var result = await _AcountService.UpdateUserAsync(userdb);
                    if (!result.Succeeded)
                    {
                        return BadRequest(result.Errors);
                    }

                    //if(userDto.Projects!=null && userDto.Projects.Count() > 0)
                    //{
                        var result1 = _AcountService.UpadteUserProjects(userdb.Id, userDto.Projects);
                        if (!result1.Succeeded)
                        {
                            return BadRequest(result1.Errors);
                        }

                    //}

                    
                    var resultData = await _AcountService.UpdateUserRolesAsync(userdb,userDto.Job );
                    if (!resultData.Succeeded)
                    {
                        return BadRequest(resultData.Errors);
                    }
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest(ModelState);

        }
        
        [HttpDelete("Delete/User/{userId}")]
        [Authorize(policy: "users.manage")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _AcountService.DeleteUserAsync(userId);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();

        }
        [HttpGet("Disable/{userId}/{Disable}")]
        [Authorize(policy: "users.manage")]
        public async Task<IActionResult> DisableUser(string userId, bool Disable)
        {
           var Result = await _AcountService.Disable(userId, Disable);
            if (!Result.Succeeded)
            {
                return BadRequest(Result.Errors);
            }
            return Ok();

        }

        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult>ChangePassword (ChangePasswordDto changePassword)
        {
            if (ModelState.IsValid)
            {
                var Result = await _AcountService.ChangePasswordAsync(changePassword.userId, changePassword.CurrentPassword, changePassword.NewPassword);

                if (!Result.Succeeded)
                {
                    return BadRequest(Result.Errors);
                }
                return Ok();
            }
            return BadRequest(ModelState);
        }
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    ApplicationUser isExistsUser = _AcountService.GetUserByPhoneNumberAsync(userLogin.PhoneNumber);
                    if (isExistsUser == null)
                    {
                        return BadRequest("PhoneNumber or Password is Invaild");
                    }
                    bool found = await _AcountService.CheckPasswordAsync(isExistsUser, userLogin.Password);
                    if (!found)
                    {
                        return BadRequest("PhoneNumber or Password is Invaild");

                    }
                    //if (isExistsUser.EmailConfirmed == false)
                    //{
                    //    return BadRequest("PhoneNumber or Password is Invaild");

                    //}

                    List<Claim> cliams = await _AcountService.SetClaimsToUserAsync(isExistsUser);

                    JwtSecurityToken Mytoken = _AcountService.GenerateToken(cliams);

                    return Ok(new
                    {

                        token = new JwtSecurityTokenHandler().WriteToken(Mytoken),
                        expired = Mytoken.ValidTo,
                        UserId = isExistsUser.Id




                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
               
            }
            return BadRequest(ModelState);


        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult>Register ([FromBody]RegisterDto registerDto)
        {
            if (ModelState.IsValid)
            {
                var trans = await _customerService.BeginTransactionAsync();
                try
                {
                    //genetate Code
                    Random generator = new Random();
                    string Code = generator.Next(0, 100000).ToString("D6");

                    ApplicationUser user = new ApplicationUser()
                    {
                        FullName = registerDto.FullName,
                        Email = registerDto.Email,
                        PhoneNumber = registerDto.PhoneNumber,
                        PasswordHash = registerDto.Password,
                        UserName = registerDto.PhoneNumber,
                        Code = Code,
                        projectId = registerDto.projectId,
                        Image = registerDto.Image != null ? registerDto.Image : "https://etsaq.com/be/Users/user.png"

                    };
                    var Result1 = await _AcountService.CreateUserAsync(user);

                    if (!Result1.Succeeded)
                    {
                        return BadRequest(Result1.Errors);
                    }

                    Customer customer = new Customer()
                    {
                        UserId = user.Id,
                        BulidingName = registerDto.BulidingName,
                        UnitNo = registerDto.UnitNo,
                        City = registerDto.City,
                        TypeProject = registerDto.TypeProject,
                    };
                    var Result3 = await _customerService.AddAsync(customer);
                    if (!Result3.Succeeded)
                    {
                        return BadRequest(Result3.Errors);
                    }
                   
                    var Result4 = await _locationService.AddLocationAsync(customer.Id, registerDto.Longitude, registerDto.Latitude);
                    if (!Result4.Succeeded)
                    {
                        return BadRequest(Result4.Errors);
                    }


                    var Result5 = await _AcountService.AssignUserToRolesAsync(user, ["عميل"]);
                    if (!Result5.Succeeded)
                    {
                        return BadRequest(Result5.Errors);
                    }

                    //Send Mail To Email 
                    //var Message = $"Code To Confirm  Email  {Code} ";
                    var Message = MessageSender2.Messages.ConfrimEmail(Code);
                    var MsResult = await _MessageSender.Send3Async(user.PhoneNumber, Message, null);
                    if (!MsResult)
                    {
                        return BadRequest("Faild To Confirm Email Try again");
                    }
                    var Result6 = await _emailService.SendEmailAsync(user.Email, Message, "Confirm Email");
                    if (!Result6.Succeeded)
                    {
                        return BadRequest("Faild To Confirm Email Try again");
                    }

                    //genarate Token to sigin to web site 
                    List<Claim> cliams = await _AcountService.SetClaimsToUserAsync(user);

                    JwtSecurityToken Mytoken = _AcountService.GenerateToken(cliams);

                    _customerService.Commit(trans);

                    return Ok(new
                    {

                        token = new JwtSecurityTokenHandler().WriteToken(Mytoken),
                        expired = Mytoken.ValidTo,
                        UserId = user.Id

                    });



                }
                catch (Exception ex)
                {
                    _customerService.Rollback(trans);
                    return BadRequest(ex.Message);

                }

            }
            return BadRequest(ModelState);
        }
        [HttpPut("Update/Data/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateData(string userId ,[FromForm]RegisterDto userDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Customer customedb = _customerService.GetCustomerByUserID(userId);
                    if (customedb == null)
                    {
                        return BadRequest("Customer Not Found");
                    }
                    if (userDto.formFile != null)
                    {
                        var result = await _fileService.ChangeImage("Users", userDto.formFile ,customedb.ApplicationUser.Image );
                        if (!result.Succeeded)
                        {
                            return BadRequest(result.Errors);

                        }

                        userDto.Image = result.imagePath;

                    }
                   
                    var Result3 = await _customerService.Update(userId,userDto);
                    if (!Result3.Succeeded)
                    {
                        return BadRequest(Result3.Errors);
                    }
                    var Result4 = _locationService.UpdateLocation(customedb.Id, userDto.Longitude, userDto.Latitude);
                    if (!Result4.Succeeded)
                    {
                        return BadRequest(Result4.Errors);

                    }



                    return Ok();


                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);

                }

            }
            return BadRequest(ModelState);

        }
        [HttpGet("GetCustomer/{UserId}")]
        [AllowAnonymous]
        public IActionResult GetCustomerByUserId (string UserId)
        
        {
            var Customer = _customerService.GetCustomerByUserID(UserId);
            if(Customer!=null)
            {
                Location Location = _locationService.GetLocationByCustomerId(Customer.Id);

                return Ok(new
                {
                    Id = Customer.Id,
                    FullName = Customer.ApplicationUser.FullName,
                    Email = Customer.ApplicationUser.Email,
                    PhoneNumber = Customer.ApplicationUser.PhoneNumber,
                    projectId = Customer.ApplicationUser.projectId!=null? Customer.ApplicationUser.projectId:0,
                    TypeProject = Customer.TypeProject, //مالك - مستأجر
                    City = Customer.City,
                    BulidingName = Customer.BulidingName,
                    UnitNo = Customer.UnitNo,
                    Image = Customer.ApplicationUser.Image,
                    Latitude = Location?.Latitude,
                    Longitude = Location?.Longitude
                });

            }
            return BadRequest("Customer Not Found");
        }
        [HttpGet("GetUser/{UserId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserByUserId(string UserId)
        {
            var user = await _AcountService.GetUserByIdAsync(UserId);
            if (user != null)
            {
                return Ok(new
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Image = user.Image,

                });
            }
            return BadRequest("User Not Found");

        }

        [HttpGet("GetUserDataToUpdate/{UserId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserAllDataByUserId(string UserId)
        {
            var user = await _AcountService.GetUserByIdAsync(UserId);
        
            if (user != null)
            {
                var RoleName = await _AcountService.GetRolesToUser(user);
                List<string> Roles = await _AcountService.GetRolesToUser(user);
                List<int> ProjectManagment = _AcountService.GetUserProjects(UserId);
                return Ok(new
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Image = user.Image,
                    projectId=user.projectId!=null? user.projectId:0,
                    Job =Roles,
                    confirm=user.EmailConfirmed,
                    Projects =ProjectManagment

                });
            }
            return BadRequest("User Not Found");

        }

        [HttpGet("SendResetPasswordCode/{EmailOrPhone}")]
        [AllowAnonymous]
        public async Task<IActionResult> SendResetPasswordCode (string EmailOrPhone)
        {
            var trans = await _customerService.BeginTransactionAsync();

            try
            {
                if (!EmailOrPhone.IsNullOrEmpty())
                {
                    ApplicationUser FindByEmail = await _AcountService.GetUserByEmailAsync2(EmailOrPhone);
                    ApplicationUser FindByPhone =  _AcountService.GetUserByPhoneNumberAsync(EmailOrPhone);
                    if (FindByEmail == null && FindByPhone==null)
                    {
                        return BadRequest("Email Or Phone Not Exists try Another Email");
                    }
                    ApplicationUser userdb=new ApplicationUser();
                    if (FindByEmail != null)
                    {
                        userdb = FindByEmail;
                    }
                   else if (FindByPhone != null)
                    {
                        userdb = FindByPhone;
                    } 
                    //genetate Code
                    Random generator = new Random();
                    string Code = generator.Next(0, 100000).ToString("D6");
                    //save Code in DB
                    var Result = await _AcountService.UpdateCodeAsync(userdb, Code);
                    if (!Result.Succeeded)
                    {
                        return BadRequest(Result.Errors);
                    }
                    //Send Mail To Email 
                    var MessageBySMS = MessageSender2.Messages.ResetPasswordBySMS(userdb.FullName, Code);
                    var Result1 = await _MessageSender.Send3Async(userdb.PhoneNumber, MessageBySMS, null);
                    if (!Result1)
                    {
                        return BadRequest("Faild To Reset Password Try again");
                    }
                    var Message = MessageSender2.Messages.ResetPassword(userdb.FullName, Code);
                    var Result2 = await _emailService.SendEmailAsync(userdb.Email, Message, "Reset Password");
                    if (!Result2.Succeeded)
                    {
                        return BadRequest("Faild To Reset Password Try again");
                    }
                    _customerService.Commit(trans);

                    return Ok();



                }
                return BadRequest("Enter The Email Please");

            }
            catch (Exception ex)
            {
                _customerService.Rollback(trans);
                return BadRequest(ex.Message);
            }
        }
 
        [HttpGet("ConfirmResetPassword/{EmailOrPhone}/{Code}")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmResetPassword(string EmailOrPhone, string Code)
        {
            if (!EmailOrPhone.IsNullOrEmpty()&& !Code.IsNullOrEmpty())
            {
                ApplicationUser FindByEmail = await _AcountService.GetUserByEmailAsync2(EmailOrPhone);
                ApplicationUser FindByPhone = _AcountService.GetUserByPhoneNumberAsync(EmailOrPhone);
                if (FindByEmail == null && FindByPhone == null)
                {
                    return BadRequest("Email Or Phone Not Exists try Another Email");
                }
                ApplicationUser userdb = new ApplicationUser();
                if (FindByEmail != null)
                {
                    userdb = FindByEmail;
                }
                else if (FindByPhone != null)
                {
                    userdb = FindByPhone;
                }
                if (userdb.Code != Code)
                {
                    return BadRequest("Invaild Code");
                }
                userdb.EmailConfirmed = true;
              var result = await  _AcountService.UpdateUserAsync(userdb);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                return Ok();

            }
            return BadRequest("Enter Email && Code");
        }
        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPassword)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser FindByEmail = await _AcountService.GetUserByEmailAsync2(resetPassword.EmailOrPhone);
                ApplicationUser FindByPhone = _AcountService.GetUserByPhoneNumberAsync(resetPassword.EmailOrPhone);
                if (FindByEmail == null && FindByPhone == null)
                {
                    return BadRequest("Email Or Phone Not Exists try Another Email");
                }
                ApplicationUser userdb = new ApplicationUser();
                if (FindByEmail != null)
                {
                    userdb = FindByEmail;
                }
                else if (FindByPhone != null)
                {
                    userdb = FindByPhone;
                }

                var Result = await _AcountService.ResetPasswordAsync(userdb, resetPassword.NewPassword);

                if (!Result.Succeeded)
                {
                    return BadRequest(Result.Errors);
                }

                return Ok();

            }
            return BadRequest("Enter Email && Code");
        }

        [HttpGet("Technicians")]
       // [AllowAnonymous]
       [Authorize(policy: "users.viewTechnicians")]
        public async Task<IActionResult>GetTechnicians()
        {
            var result = await _AcountService.GetUsersByRole("فنى");

            if (!result.Succeeded)
            {
                return BadRequest("Faild To Load Technicians");
            }
            List<UserBasicInfoDto> Technicians = result.Users;
            return Ok(Technicians);
        }
        [HttpGet("Clients")]
        [Authorize(policy: "users.viewClients")]
        public async Task<IActionResult> GetClients()
        {
            var result = await _AcountService.GetUsersByRole("عميل");

            if (!result.Succeeded)
            {
                return BadRequest("Faild To Load Clients");
            }
            List<UserBasicInfoDto> Technicians = result.Users;
            return Ok(Technicians);
        }
        [HttpGet("projectEmployees/{proId}")]
        [Authorize(policy: "users.viewClients")]
        public IActionResult GetEmployeesByProject(int proId)
        {
          return Ok(_AcountService.GetEmployeesByProject(proId));


        }

        [HttpGet("Managers")]
        [Authorize(policy: "users.viewTechnicians")]
        public async Task<IActionResult> GetManagers()
        {
            var result = await _AcountService.GetUsersByRole("مشرف");

            if (!result.Succeeded)
            {
                return BadRequest("Faild To Load Managers");
            }
            List<UserBasicInfoDto> Managers = result.Users;
            return Ok(Managers);
        }

        [HttpGet("Users/{page}/{pageSize}")]
        [Authorize(policy: "users.view")]
        public  async Task< IActionResult> Users(string? search, string? role , int page=1 ,int pageSize = 10)
        {
            var users = await _AcountService.GetUsers(search, role).ToPaginatedListAsync(page,pageSize);
            //var userByRoles = await _AcountService.GetUsersRole(users);
            return Ok(users);

        }

        [HttpGet("ConfirmEmail/{Email}/{Code}")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string Email ,string Code  )
        {
            var user = await _AcountService.GetUserByEmailAsync2(Email);
            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            if (user.Code == Code)
            {
                user.EmailConfirmed = true;
               var result =  await _AcountService.UpdateCodeAsync(user, Code);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return Ok();
            }
            return BadRequest("Invaild Code");


        }


        [HttpGet("ClientReceiving/{userId}")]
        [Authorize]
        public async Task<IActionResult> ClientReceiving(string userId)
        {
            Client clientdb =await _customerService.GetClientReservation(userId);

            return Ok(clientdb);

        }
        [HttpGet("ClientsReceiving")]
        [Authorize(policy: "ClientsReceiving.manage")]
        public async Task<IActionResult> ClientsReceiving(int page=1,int pageSize=10,string projectName=null,string buildingName=null,string ClientName=null,string ClientPhone=null,DateOnly from=default,DateOnly to=default)
        {
          var clients = await _customerService.ClientsReservation(projectName,buildingName,ClientName,ClientPhone,from,to).ToPaginatedListAsync(page,pageSize);

            return Ok(clients);

        }


        [HttpGet("SendConfirmationCode/{id}")]
        [Authorize]
        public async Task<IActionResult> SendConfirmationCode(int id)
        {
            try
            {
                var result = await _customerService.SendRecervationCode(id);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("ConfirmationCode/{id}/{Code}")]
        [Authorize]
        public async Task<IActionResult> ConfirmationCode(int id, string Code)
        {
            try
            {
                var result = await _customerService.ConfirmReservation(id,Code);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("User/Projects/{userId}")]
       // [Authorize]
        [AllowAnonymous]
        public IActionResult GetUserProject(string userId)
        {
            List<Project> projects = new List<Project>();
            projects = _AcountService.GetUserProjectsList(userId);
            if (projects.Count() == 0)
            {
                projects = _ProjectService.GetProjects();
            }
            return Ok(projects);

        }



    }


}
