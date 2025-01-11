using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EtisiqueApi.Repositiories
{
	public class ProjectService : GenaricService<Project>, IProjectService
	{
		private readonly DbSet<Project> _Project;
		public ProjectService(Context context) :base(context)
		{
            _Project = context.Set<Project>();
		}

        public IQueryable<ProjectResponse> GetAllProjects(string? search)
        {
           if(search == null)
            {
                return _Project.AsNoTracking().Include(p => p.CreatedBy)
                 .OrderByDescending(p=>p.Id)
                 .Where(p=>p.IsDeleted==false)
               .Select(p => new ProjectResponse()
               {
                   Id = p.Id,
                   ProjectName = p.ProjectName,
                   ProjectStatus = p.ProjectStatus == 1 ? "متوفر" : "غير متوفر",
                   BuidingNo = p.BuidingNo,
                   CreatedDate = p.CreatedDate.ToString(),
                   UpdatedDate = p.UpdatedDate.ToString(),
                   City = p.City,
                   UnitNo = p.UnitNo,
                   ImageUrl = p.ImageUrl,
                   Neighborhood = p.Neighborhood,
                   CreatedBy = p.CreatedBy.FullName,
                   Address = p.Address,
                   Description = p.Description

               }).AsQueryable();
            }
            return _Project.AsNoTracking().Include(p => p.CreatedBy)
                .OrderByDescending(p => p.CreatedDate)
                .Where(p => p.IsDeleted == false)
                .Where(P => P.ProjectName.Contains(search)).Select(p => new ProjectResponse()
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    ProjectStatus = p.ProjectStatus == 1 ? "متوفر" : "غير متوفر",
                    BuidingNo = p.BuidingNo,
                    CreatedDate = p.CreatedDate.ToString(),
                    UpdatedDate = p.UpdatedDate.ToString(),
                    City = p.City,
                    UnitNo = p.UnitNo,
                    ImageUrl = p.ImageUrl,
                    Neighborhood = p.Neighborhood,
                    CreatedBy = p.CreatedBy.FullName,
                    Address = p.Address,
                    Description = p.Description

                }).AsQueryable();
        }

        public override async Task<Project> GetByIdAsync(int id)
        {
			return await _Project.Include(p => p.CreatedBy).FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);
        }

        public List<Project> GetProjects()
        {
            return _Project.AsNoTracking().Where(p=>p.IsDeleted==false && p.ProjectStatus==1).Select(P=>new Project{ ProjectName = P.ProjectName, Id = P.Id } ).ToList();
        }

    }
}
