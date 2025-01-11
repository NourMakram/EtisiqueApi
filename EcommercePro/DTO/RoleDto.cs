using EtisiqueApi.Models;
using System.ComponentModel.DataAnnotations;

namespace EcommercePro.DTO
{
	public class RoleDto
	{
		public string Id { get; set; }
		[Required(ErrorMessage = "Role Name is Reqiured")]
		[UniqueRole]
		public string Name { get; set; }
		public string? Description { get; set; }

		public List<string> Permissions { get; set; }

	}
}
