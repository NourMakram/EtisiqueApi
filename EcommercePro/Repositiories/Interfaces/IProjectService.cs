using EcommercePro.Models;
using EtisiqueApi.DTO;

namespace EtisiqueApi.Repositiories.Interfaces
{
	public interface IProjectService:IGenaricService<Project>
	{
		public List<Project> GetProjects();
		public IQueryable<ProjectResponse> GetAllProjects(string? search);
	}
}
