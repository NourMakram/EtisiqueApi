using EcommercePro.Models;

namespace EcommercePro.DTO
{
	public class PermissionDto
	{

		public string Value { get; set; }


		public static explicit operator PermissionDto(ApplicationPermission permission)
		{
			return new PermissionDto
			{

				Value = permission?.Value,

			};
		}
	}
	public class PermissionList
	{
		public string GroupName { get; set; }
		public List<Permission> Permissions { get; set; }

	}
	public class Permission{
		public string name { get; set; }
		public string value { get; set; }
		public bool Checked { get; set; }	

		}
}
	