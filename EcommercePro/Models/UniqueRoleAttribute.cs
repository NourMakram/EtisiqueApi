using EcommercePro.DTO;
using EcommercePro.Models;
using EtisiqueApi.DTO;
using System.ComponentModel.DataAnnotations;

namespace EtisiqueApi.Models
{
    public class UniqueRoleAttribute:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return null;
            }
            string newValue = value.ToString();
            Context context = validationContext.GetService<Context>();


            if (validationContext.ObjectType == typeof(RoleDto))
            {
                RoleDto Role = (RoleDto)validationContext.ObjectInstance;
                ApplicationRole RoleDb = context.Roles.FirstOrDefault(role => role.Name == newValue  && role.IsDeleted == false);
                if (RoleDb != null && (RoleDb.Id != Role.Id))
                {
                    return new ValidationResult("هذا الدور موجود مسبقا");
                }

            }


            return ValidationResult.Success;
        }
    }
}
