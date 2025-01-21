using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Pagination;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Drawing;

namespace EtisiqueApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProjectController : ControllerBase
	{
		private IHttpContextAccessor _contextAccessor;
		private IProjectService _projectService;
		private IFileService _fileService;
		public ProjectController(IHttpContextAccessor contextAccessor ,
            IProjectService projectService, 
			IFileService fileService )
		{
			_contextAccessor = contextAccessor;
            _projectService = projectService;
			_fileService = fileService;

		}
		[HttpPost("Create")]
        [Authorize(policy: "projects.manage")]
         public async Task<IActionResult> Add([FromForm]ProjectDto project)
		{
			if (ModelState.IsValid) {
				if (project.FileUrl != null)
				{
					var resultimage =await _fileService.SaveImage("Project", project.FileUrl);
					if (!resultimage.Succeeded)
					{
						return BadRequest(resultimage.Errors);
					}
					project.ImageUrl = resultimage.imagePath;

				}
                var currentDate = DateTime.UtcNow;

                // Increase the hour by 3
                var dateAfter3Hours = currentDate.AddHours(3);
                Project newProject = new Project()
				{
					ProjectName = project.ProjectName,
					Address = project.Address,
					BuidingNo = project.BuidingNo,
					UnitNo = project.UnitNo,
					City = project.City,
					Neighborhood = project.Neighborhood,
					Description = project.Description,
					CreatedById = project.CreatedById,
					ImageUrl= project.ImageUrl,
					CreatedDate = dateAfter3Hours,
					hasGuarantee = project.hasGuarantee

				};
				var result = await _projectService.AddAsync(newProject);
				if (!result.Succeeded)
				{
                       return BadRequest(result.Errors);
				}
				if(project.formFiles!=null && project.formFiles.Count() > 0)
				{
					var result2 = await _projectService.SetProjectImages(project.formFiles, newProject.Id);
					if (!result2.Succeeded)
					{
						return BadRequest(result2.Errors);
					}
				}
				
				return Ok();
             }
			return BadRequest(ModelState);

		}

		[HttpPut("{ProjectId:int}")]
        [Authorize(policy: "projects.manage")]
         public async Task<IActionResult> Update(int ProjectId , [FromForm]ProjectDto project )
		{
			if (ModelState.IsValid)
			{
				Project projectDb = await _projectService.GetByIdAsync(ProjectId);
				if (projectDb == null)
				{
					return BadRequest("Project Not Found");

				}
				if (project.FileUrl != null)
				{
					var result = await _fileService.ChangeImage("Project", project.FileUrl, projectDb.ImageUrl);
					if (!result.Succeeded)
					{
						return BadRequest(result.Errors);
					}
					project.ImageUrl = result.imagePath;

				}
				else
				{
					project.ImageUrl = projectDb.ImageUrl;
				}
                var currentDate = DateTime.UtcNow;

                // Increase the hour by 3
                var dateAfter3Hours = currentDate.AddHours(3);

                    projectDb.Id = ProjectId;
					projectDb.ProjectName = project.ProjectName;
					projectDb.Address = project.Address;
					projectDb.BuidingNo = project.BuidingNo;
					projectDb.UnitNo = project.UnitNo;
					projectDb.City = project.City;
					projectDb.Neighborhood = project.Neighborhood;
					projectDb.Description = project.Description;
 					projectDb.ImageUrl = project.ImageUrl;
					projectDb.CreatedDate = projectDb.CreatedDate;
                    projectDb.UpdatedDate = dateAfter3Hours;
				    projectDb.hasGuarantee = project.hasGuarantee;

					var Result  = _projectService.Update(projectDb);
					if (!Result.Succeeded)
					{
						return BadRequest(Result.Errors);
					}
					if (project.formFiles != null && project.formFiles.Count() > 0)
					{
						var result2 = await _projectService.SetProjectImages(project.formFiles, projectDb.Id);
						if (!result2.Succeeded)
						{
							return BadRequest(result2.Errors);
						}
					}
				return Ok();
				
			}
			return BadRequest(ModelState);

		}

		[HttpDelete("SoftDelete/{projectId:int}")]
        [Authorize(policy: "projects.manage")]
 
        public async Task<IActionResult> SoftDelete(int projectId)
		{
			Project projectDb = await _projectService.GetByIdAsync(projectId);

			if (projectDb != null)
			{
				projectDb.IsDeleted = true;
				var result = _projectService.Update(projectDb);
				if (!result.Succeeded)
				{
					return BadRequest(result.Errors);
				}
				return Ok();

			}
			return BadRequest("project Not Found");

		}
        [HttpDelete("Delete/{projectId:int}")]
        [Authorize(policy: "projects.manage")]
 
        public async Task<IActionResult> Delete(int projectId)
        {
            Project projectDb = await _projectService.GetByIdAsync(projectId);

            if (projectDb != null)
            {
                var result = _projectService.Delete(projectDb);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                return Ok();

            }
            return BadRequest("project Not Found");

        }
        [HttpGet("{projectId:int}/{Status:int}")]
        [Authorize(policy: "projects.manage")]
  
        public async Task<IActionResult> ChangeStatus(int projectId ,int Status)
		{
			Project projectDb = await _projectService.GetByIdAsync(projectId);

			if (projectDb != null && (Status==1 || Status==2 ) )
			{
				projectDb.ProjectStatus = Status;
				var result = _projectService.Update(projectDb);
				if (!result.Succeeded)
				{
					return BadRequest(result.Errors);
				}
				return Ok();

			}
			return BadRequest("project Not Found");

		}

		[HttpGet("Projects")]
		[AllowAnonymous]
		public IActionResult GetProjects()
		{
			var Projects = _projectService.GetProjects().Select(p=>new {p.Id,p.ProjectName});
			return Ok(Projects);
		}

		[HttpGet("Projects/Paginted/{pageNum}/{pageSize}")]
        [Authorize(policy: "projects.view")]
         public async Task<IActionResult> GetProjectsPaginted(string? search,int pageNum=1, int pageSize=10)
        {
            var Projects =await _projectService.GetAllProjects(search).ToPaginatedListAsync(pageNum,pageSize);
            return Ok(Projects);
        }
       
		[HttpGet("Project/{projectId}")]
        [Authorize(policy: "projects.view")]
 		public async Task<IActionResult> GetProject(int projectId)
		{
            Project projectDb = await _projectService.GetByIdByInclude(projectId);

            if (projectDb == null)
            {
                return BadRequest("project Not Found");
            }
			return Ok(new ProjectResponse()
			{
                Id = projectDb.Id,
                ProjectName = projectDb.ProjectName,
                ProjectStatus = projectDb.ProjectStatus == 1 ? "متوفر" : "غير متوفر",
                BuidingNo = projectDb.BuidingNo,
                CreatedDate = projectDb.CreatedDate.ToString(),
                UpdatedDate = projectDb.UpdatedDate.ToString(),
                City = projectDb.City,
                UnitNo = projectDb.UnitNo,
                ImageUrl = projectDb.ImageUrl,
                Neighborhood = projectDb.Neighborhood,
                CreatedBy = projectDb.CreatedBy.FullName,
                Address = projectDb.Address,
                Description = projectDb.Description,
				images=projectDb.ProjectImages.Select(i=>i.imagePath).ToList(),
            });
        }

    }



}
