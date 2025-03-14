using EntityFrameworkCore.EncryptColumn.Extension;
using EntityFrameworkCore.EncryptColumn.Interfaces;
using EntityFrameworkCore.EncryptColumn.Util;
using EtisiqueApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Stripe.Terminal;
using System.Diagnostics.Contracts;

namespace EcommercePro.Models
{
	public class Context : IdentityDbContext<ApplicationUser, ApplicationRole, string>
	{
       // private readonly IEncryptionProvider _encryptionProvider;
        public Context(DbContextOptions option) : base(option)
        {

         }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectImages> ProjectImages { set; get; }
		public DbSet<Location> Locations { get; set; }
		public DbSet<Contract> Contracts { get; set; }
		public DbSet<Guarantee> Guarantees { get; set; }
		public DbSet<Customer> Customers { get; set; }
        public DbSet<ApartmentService> ApartmentServices { get; set; }
        public DbSet<ApartmentServicesType> ApartmentServicesTypes { get; set; }
        public DbSet<ApartmentSubServicesRequest> ApartmentSubServicesRequests { get; set; }
        public DbSet<ApartmentServicesVerifications> ApartmentServicesVerifications { set; get; }
        public DbSet<ApartmentServicesRequest> ApartmentServicesRequests { get; set; }
        public DbSet<CarWashRequest> CarWashRequests { get; set; }
        public DbSet<CarWashContract> CarWashContracts { get; set; }
        public DbSet<RequestManagement> RequestManagements { get; set; }
        public DbSet<RequestsCommonParts> RequestCommonParts { get; set; }
        public DbSet<CommonPartsVerifications> CommonPartsVerifications { set; get; }
        public DbSet<KitchenServices> KitchenServices { get; set; }
        public DbSet<TechnicianQusetions> TechnicianQusetions { get; set; }
        public DbSet<Questionnaire> Questionnaires {get; set; }
        public DbSet<DaysOff> DaysOffs { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
       public DbSet<Rule> Rules { set; get; }
        public DbSet<Shrt> Shrts { set; get; }
        public DbSet<UserProject> UserProjects { get; set; }
        public DbSet<Emergency> Emergencies { set; get; }
        public DbSet<RequestsImages> RequestsImages { get; set; }
        public DbSet<Attchments> Attchments { set; get; }
        public DbSet<EmergencyTypes> EmergencyTypes { set; get; }
        public DbSet<CompleteDays> CompleteDays { set; get; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationRole>()
                .HasMany(r => r.Claims)
                .WithOne()
                .HasForeignKey(c => c.RoleId)
                .IsRequired();

            modelBuilder.Entity<Project>()
             .HasOne(p => p.CreatedBy)
             .WithMany()
             .HasForeignKey(p => p.CreatedById);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Project)
                .WithMany()
                .HasForeignKey(u => u.projectId);


            //modelBuilder.Entity<ApartmentServices>().HasData(
            //new ApartmentServices() { Id = 1, Name = "شقق" },
            //new ApartmentServices() { Id = 2, Name = "غسيل السيارت" },
            //new ApartmentServices() { Id = 3, Name = "الباقات" },
            //new ApartmentServices() { Id = 4, Name = "المتجر" },
            //new ApartmentServices() { Id = 5, Name = "الموسمية" },
            //new ApartmentServices() { Id = 6, Name = "مبيدات حشرية" }
            //  );
            modelBuilder.Entity<EmergencyTypes>().HasData(
                 new EmergencyTypes() { Id = 1, Name = "انقطاع ماء" },
                 new EmergencyTypes() { Id = 2, Name = "توقف مصعد" },
                 new EmergencyTypes() { Id = 3, Name = "انقطاع كهرباء" },
                  new EmergencyTypes() { Id = 4, Name = "التماس كهرباء" }
                );
            
        modelBuilder.Entity<ApartmentService>().HasData(
                new ApartmentService() { Id = 1, Name = "كهرباء" },
                new ApartmentService() { Id = 2, Name = "سباكة" },
                new ApartmentService() { Id = 3, Name = "دهان" },
                new ApartmentService() { Id = 4, Name = "رخام أو بلاط" },
                new ApartmentService() { Id = 5, Name = "المنيوم وزجاج" },
                new ApartmentService() { Id = 6, Name = "خشب" },
                new ApartmentService() { Id = 7, Name = "جبس" },
                new ApartmentService() { Id = 8, Name = "نحاس تكييف" },
                new ApartmentService() { Id = 9, Name = "انتركوم" },
		        new ApartmentService() { Id = 10, Name = "نظافة" }

				);

            modelBuilder.Entity<ApartmentServicesType>().HasData(
             new ApartmentServicesType() { Id = 1, Name = "ليد", ApartmentServiceId = 1 },
             new ApartmentServicesType() { Id = 2, Name = "سبوت لايت", ApartmentServiceId = 1 },
             new ApartmentServicesType() { Id = 3, Name = "مروحة شفط", ApartmentServiceId = 1 },
             new ApartmentServicesType() { Id = 4, Name = "لمبة سطح", ApartmentServiceId = 1 },
             new ApartmentServicesType() { Id = 5, Name = "مفتاح", ApartmentServiceId = 1 },
             new ApartmentServicesType() { Id = 6, Name = "مفتاح طبلون", ApartmentServiceId = 1 },
             new ApartmentServicesType() { Id = 7, Name = "التماس", ApartmentServiceId = 1 },
             new ApartmentServicesType() { Id = 8, Name = "أخرى", ApartmentServiceId = 1 },

             new ApartmentServicesType() { Id = 9, Name = "سخان", ApartmentServiceId = 2 },
             new ApartmentServicesType() { Id = 10, Name = "خلاط", ApartmentServiceId = 2 },
             new ApartmentServicesType() { Id = 11, Name = "شطاف", ApartmentServiceId = 2 },
             new ApartmentServicesType() { Id = 12, Name = "دش مطرى", ApartmentServiceId = 2 },
             new ApartmentServicesType() { Id = 13, Name = "سيفون", ApartmentServiceId = 2 },
             new ApartmentServicesType() { Id = 14, Name = "تمديدات", ApartmentServiceId = 2 },
             new ApartmentServicesType() { Id = 15, Name = "أخرى", ApartmentServiceId = 2 },

             new ApartmentServicesType() { Id = 16, Name = "جدار", ApartmentServiceId = 3 },
             new ApartmentServicesType() { Id = 17, Name = "سقف", ApartmentServiceId = 3 },
             new ApartmentServicesType() { Id = 18, Name = " باب حديد", ApartmentServiceId = 3 },
             new ApartmentServicesType() { Id = 19, Name = "بروفايل", ApartmentServiceId = 3 },
             new ApartmentServicesType() { Id = 20, Name = "أخرى", ApartmentServiceId = 3 },

             new ApartmentServicesType() { Id = 21, Name = "غرفة", ApartmentServiceId = 4 },
             new ApartmentServicesType() { Id = 22, Name = "سطح", ApartmentServiceId = 4 },
             new ApartmentServicesType() { Id = 23, Name = "حوش", ApartmentServiceId = 4 },
             new ApartmentServicesType() { Id = 24, Name = "دورة مياه", ApartmentServiceId = 4 },

             new ApartmentServicesType() { Id = 25, Name = "قفل شباك", ApartmentServiceId = 5 },
             new ApartmentServicesType() { Id = 26, Name = "مسكة شباك", ApartmentServiceId = 5 },
             new ApartmentServicesType() { Id = 27, Name = "شبك بعوض", ApartmentServiceId = 5 },
             new ApartmentServicesType() { Id = 28, Name = "باب زجاج", ApartmentServiceId = 5 },
             new ApartmentServicesType() { Id = 29, Name = "مفصلات زجاج", ApartmentServiceId = 5 },
             new ApartmentServicesType() { Id = 30, Name = "اخرى", ApartmentServiceId = 5 }
             );


        }


    }
}
