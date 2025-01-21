using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Stripe.Terminal;

namespace EtisiqueApi.Repositiories
{
	public class ProjectService : GenaricService<Project>, IProjectService
	{
		private readonly Context _dbContext;
		private IFileService _fileService;
		private IHttpContextAccessor _contextAccessor;

		public ProjectService(Context context, IFileService fileService, IHttpContextAccessor contextAccessor) :base(context)
		{
            _dbContext= context;
			_fileService = fileService;
			_contextAccessor = contextAccessor;
		}

        public IQueryable<ProjectResponse> GetAllProjects(string? search)
        {
           if(search == null)
            {
                return _dbContext.Projects.AsNoTracking().Include(p => p.CreatedBy)
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
            return _dbContext.Projects.AsNoTracking().Include(p => p.CreatedBy)
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
			return await _dbContext.Projects.Include(p => p.CreatedBy)
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);
        }
		public async Task<Project> GetByIdByInclude(int id)
		{
			return await _dbContext.Projects.Include(p => p.CreatedBy)
                .Include(p=>p.ProjectImages)
				.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);
		}
		public List<Project> GetProjects()
        {
            return _dbContext.Projects.AsNoTracking()
                .Where(p=>p.IsDeleted==false && p.ProjectStatus==1)
                .Select(P=>new Project{ ProjectName = P.ProjectName, Id = P.Id } ).ToList();
        }

		public async  Task<(bool Succeeded, string[] Errors)> SetProjectImages(List<IFormFile> files , int proId)
		{
			try
			{
				var context = _contextAccessor.HttpContext.Request;
				var baseurl = context.Scheme + "://" + context.Host;

				List<ProjectImages> images = new List<ProjectImages>();
				foreach (IFormFile file in files)
				{
					var result = await _fileService.SaveImage2("Project", file, baseurl);
					if (result.Succeeded)
					{
						images.Add(new ProjectImages()
						{
							ProjectId = proId,
							imagePath = result.imagePath
 						});
					}
					else
					{
						return (false, result.Errors);

					}
				}
				_dbContext.ProjectImages.AddRange(images);
				_dbContext.SaveChanges();
				return (true, null);
			}
			catch (Exception ex)
			{
				return (false, new string[] { ex.Message });
			}
		}

		public async Task<(bool Succeeded, string[] Errors)> UpdateProjectImages(List<IFormFile> files, int proId)
		{
			try
			{
				var OldProjectImages = _dbContext.ProjectImages.Where(i => i.ProjectId == proId);
				if (OldProjectImages != null)
				{
					_dbContext.ProjectImages.RemoveRange(OldProjectImages);

				}
				var context = _contextAccessor.HttpContext.Request;
				var baseurl = context.Scheme + "://" + context.Host;

				List<ProjectImages> images = new List<ProjectImages>();
				foreach (IFormFile file in files)
				{
					var result = await _fileService.SaveImage2("Project", file, baseurl);
					if (result.Succeeded)
					{
						images.Add(new ProjectImages()
						{
							ProjectId = proId,
							imagePath = result.imagePath
						});
					}
					else
					{
						return (false, result.Errors);

					}
				}
				_dbContext.ProjectImages.AddRange(images);
				_dbContext.SaveChanges();
				return (true, null);
			}
			catch (Exception ex)
			{
				return (false, new string[] { ex.Message });
			}
		}
	}
}
