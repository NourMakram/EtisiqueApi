using EcommercePro.Models;
using EtisiqueApi.DTO;
using System.ComponentModel.DataAnnotations;

namespace EtisiqueApi.Models
{
    public class UniqueEmailAttribute:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return null;
            }
            string newValue = value.ToString();
            Context context = validationContext.GetService<Context>();

           
            if (validationContext.ObjectType == typeof(updateDto))
            {
                updateDto updateUser = (updateDto)validationContext.ObjectInstance;
                ApplicationUser userdb = context.Users.FirstOrDefault(user => user.Email == newValue);
                if (userdb != null && (userdb.Id != updateUser.Id))
                {
                    return new ValidationResult("البريد الألكترونى موجود بالفعل قم بتسجيل الدخول او ادخل رقماَ اخر");
                }

            }
            
            
            return ValidationResult.Success;
        }
    }
}
