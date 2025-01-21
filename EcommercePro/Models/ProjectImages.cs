using EcommercePro.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.Models
{
	public class ProjectImages
	{
		public int Id { get; set; }

		[ForeignKey("Project")]
		public int ProjectId { get; set; }
		public Project? Project { get; set; }

		public string imagePath { get; set; }

	}
}
