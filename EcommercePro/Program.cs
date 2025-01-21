using EcommercePro.Hubs;
using EcommercePro.Models;
using EcommercePro.Repositiories;
using EcommercePro.Repositiories.Interfaces;
using EtisiqueApi.FilterPermissions;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSignalR();
        

        #region inject repository 
        builder.Services.AddTransient<IRoleService, RoleService>();
        builder.Services.AddTransient<IProjectService, ProjectService>();
        builder.Services.AddTransient<IContractService, ContractService>();
        builder.Services.AddTransient<IFileService , FileService>();
        builder.Services.AddTransient<IGuaranteeService, GuaranteeService>();
        builder.Services.AddTransient<IAcountService, AcountServiceL>();
        builder.Services.AddTransient<ISubServiceRequestService, subServiceRequestService>();
        builder.Services.AddTransient<ICommonPartsService, CommonPartsService>();
        builder.Services.AddTransient<ILocationService,EtisiqueApi.Repositiories.LocationService>();
        builder.Services.AddTransient<ICustomerService, CustomerService>();
        builder.Services.AddTransient<IEmailService, EmailService>();
        builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        builder.Services.AddTransient<IApartmentServicesRequestService, ApartmentServicesRequestService>();
        builder.Services.AddTransient<IRequestManagement, RequestManagmentService>();
        builder.Services.AddScoped<IKitchenService, KitchenService>();
        builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        builder.Services.AddScoped<MessageSender2.IMessageSender, MessageSender2.MessageSender>();
        builder.Services.AddScoped<IQuestionnaireService, QuestionnaireService>();
        builder.Services.AddScoped<IGenaricService<DaysOff>, GenaricService<DaysOff>>();
        builder.Services.AddScoped<IShortCutService, ShortCutService>();
        builder.Services.AddScoped<IComplaintService, ComplaintService>();
        builder.Services.AddScoped<IEmergencyService, EmergencyService>();
        builder.Services.AddScoped<IRequestImage, RequestImage>();



        builder.Services.Configure<SecurityStampValidatorOptions>(options =>
        {
            options.ValidationInterval = TimeSpan.Zero;
        });

        #endregion

        builder.Services.AddDbContext<Context>(option =>
        {
            option.UseSqlServer(builder.Configuration.GetConnectionString("DC"));
        });

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
             });
        });

        #region Authentication
        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequireUppercase = false;
            options.Password.RequiredUniqueChars = 0;
            options.Password.RequireNonAlphanumeric=false;

         })
       .AddEntityFrameworkStores<Context>();


        builder.Services.AddAuthentication(options =>
        {
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = "http://localhost:5261",
                ValidateAudience = true,
                ValidAudience = "http://localhost:4200",
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("1s3r4e5g6h7j81s3r4e5g6h7j81s3r4e5g6h7j81s3r4e5g6h7j99")
                )
            };
        });

        #endregion

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseCors();

        app.UseStaticFiles();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapHub<NotificationHub>("/NotificationHub");

      
        app.MapControllers();

        app.Run();
    }
}
