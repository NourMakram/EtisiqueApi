using EcommercePro.Models;
using EtisiqueApi.DTO;

namespace EtisiqueApi.Repositiories.Interfaces
{
	public interface IProjectService:IGenaricService<Project>
	{
		public List<Project> GetProjects();
		public IQueryable<ProjectResponse> GetAllProjects(string? search);
		public  Task<(bool Succeeded, string[] Errors)> SetProjectImages(List<IFormFile> files, int proId);

		public Task<(bool Succeeded, string[] Errors)> UpdateProjectImages(List<IFormFile> files, int proId);
		public Task<Project> GetByIdByInclude(int id);

	}
}
