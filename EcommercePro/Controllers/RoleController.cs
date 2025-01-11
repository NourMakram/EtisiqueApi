using EcommercePro.DTO;
using EcommercePro.Models;
using EcommercePro.Repositiories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using RestSharp.Extensions;
using System.Drawing;

namespace EcommercePro.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RoleController : ControllerBase
	{
		private IRoleService _authService;
		private RoleManager<ApplicationRole> _roleManager;
		public RoleController(IRoleService authService  , RoleManager<ApplicationRole> roleManager)
		{
			_authService = authService;
			_roleManager = roleManager;

		}
		[HttpPost("Role")]
		[Authorize(policy : "roles.manage")]
 		public async Task<IActionResult> CreateRole( RoleDto role)
		{
			if (ModelState.IsValid)
			{
				if (role == null)
					return BadRequest($"{nameof(role)} cannot be null");

				ApplicationRole appRole = new ApplicationRole()
				{
					Name = role.Name,
					Description = role.Description,
					CreatedDate = DateOnly.FromDateTime(DateTime.Now),
				};


				var result = await _authService.CreateRoleAsync(appRole, role.Permissions?.ToArray());
				if (result.Succeeded)
				{
					return Ok();
					//RoleViewModel roleVM = await GetRoleViewModelHelper(appRole.Name);
					//return CreatedAtAction(GetRoleByIdActionName, new { id = roleVM.Id }, roleVM);
				}

				return BadRequest(result.Errors);
			}

			return BadRequest(ModelState);
		}
        [HttpGet("GetRoles")]
        [Authorize(policy: "roles.view")]
        public async Task<IActionResult> GetRoles(int page = 1 , int Size =5)
		{
          List<ApplicationRole> Roles = await _authService.GetRolesAsync(page, Size);
			List<RoleDto> RolesDto = Roles.Select(Role =>new RoleDto()
			{
				Id = Role.Id ,
				Name=Role.Name,
				Description = Role.Description,

			}).ToList();

			return Ok(RolesDto);

		}

		[HttpGet("GetRole/{RoleId}")]
        //  [Authorize(policy: "roles.view")]
        [AllowAnonymous]

        public async Task<IActionResult> GetRole(string RoleId)
		{
			
			RoleDto RoleDatiels = await _authService.GetRoleAsync(RoleId);
			if(RoleDatiels!=null)
			  return Ok(RoleDatiels);
			return BadRequest("Role Not Found");
		}
		[HttpGet("Permissions")]
		//[Authorize(policy: "roles.manage")]
		[AllowAnonymous]
        public IActionResult GetPermissions()
		{
			List<PermissionList> permissions = _authService.GetPermissions();
			return Ok(permissions);
		}

		[HttpPut("Role/{RoleId}")]
		[Authorize(policy: "roles.manage")]
         public async Task<IActionResult> UpdateRole(string RoleId,  RoleDto role)
		{
			if (ModelState.IsValid)
			{
				if (role == null)
					return BadRequest($"{nameof(role)} cannot be null");

				ApplicationRole appRole = new ApplicationRole()
				{
					Id=role.Id,
					Name = role.Name,
					Description = role.Description,
					UpdatedDate = DateOnly.FromDateTime(DateTime.Now),
				};


				var result = await _authService.UpdateRoleAsync(RoleId,appRole, role.Permissions?.ToArray());
				if (result.Succeeded)
				{
					return Ok();
					//RoleViewModel roleVM = await GetRoleViewModelHelper(appRole.Name);
					//return CreatedAtAction(GetRoleByIdActionName, new { id = roleVM.Id }, roleVM);
				}

				return BadRequest(result.Errors);
			}

			return BadRequest(ModelState);

		}
		[HttpDelete("Role/{RoleId}")]
        [Authorize(policy: "roles.manage")]
         public async Task<IActionResult> DeleteRole(string RoleId)
		{
			var result =await  _authService.DeleteRoleAsync(RoleId);
			if (result.Succeeded)
			{
				return Ok();
			}
			return BadRequest(result.Errors);

		}

		[HttpGet("Permissions/{userId}")]
		//[Authorize(policy: "roles.manage")]
		[AllowAnonymous]
        public async Task<IActionResult>GetPermissionByUserId (string userId)
		{
              var result = await _authService.GetPermissionsByUserId(userId);
			if (!result.Succeeded)
			{
				return BadRequest("Not Found the Role");
			}
			return Ok(result.permissions);

		}
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> returnRole()
		{
			var ressult = await _roleManager.FindByNameAsync("عميل");
			ressult.IsDeleted = false;
			var result =await _roleManager.UpdateAsync(ressult);
			return Ok(result);
		}
        


	}
}

