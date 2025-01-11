using EcommercePro.Models;
using EtisiqueApi.DTO;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EtisiqueApi.Models
{
    public class UniqueAttribute:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return null;
            }
            string newValue = value.ToString();
            Context context = validationContext.GetService<Context>();

            if (validationContext.ObjectType == typeof(UserDto) )
            {
                UserDto newUser = (UserDto)validationContext.ObjectInstance;
                ApplicationUser userdb = context.Users.FirstOrDefault(user => user.PhoneNumber == newValue);
                if (userdb != null&&(userdb.Id != newUser.Id))
                {
                    return new ValidationResult(" رقم الجوال موجود بالفعل قم بتسجيل الدخول او ادخل رقماَ اخر");
                }
            
            }
            if (validationContext.ObjectType == typeof(updateDto))
            {
                updateDto updateUser = (updateDto)validationContext.ObjectInstance;
                ApplicationUser userdb = context.Users.FirstOrDefault(user => user.PhoneNumber == newValue);
                if (userdb != null && (userdb.Id != updateUser.Id))
                {
                    return new ValidationResult(" رقم الجوال موجود بالفعل قم بتسجيل الدخول او ادخل رقماَ اخر");
                }

            }
            if (validationContext.ObjectType == typeof(RegisterDto))
            {
                RegisterDto newUser = (RegisterDto)validationContext.ObjectInstance;
                Customer customerdb = context.Customers.Include(C=>C.ApplicationUser)
                    .FirstOrDefault(user => user.ApplicationUser.PhoneNumber == newValue);

                if (customerdb != null && (customerdb.Id != newUser.Id))
                {
                    return new ValidationResult(" رقم الجوال موجود بالفعل قم بتسجيل الدخول او ادخل رقماَ اخر");
                }

            }
            if (validationContext.ObjectType == typeof(ProjectDto))
            {
                ProjectDto newProject = (ProjectDto)validationContext.ObjectInstance;
                Project ProjectDb = context.Projects.FirstOrDefault(project => project.ProjectName == newValue&&project.IsDeleted ==false);
                if (ProjectDb != null && (ProjectDb.Id != newProject.Id))
                {
                    return new ValidationResult("اسم المشروع موجود بالفعل ادخل اسماَ اخر");
                }

            }
            return ValidationResult.Success;
        }
    }
}
