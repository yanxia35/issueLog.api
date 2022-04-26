using Microsoft.AspNetCore.Mvc;
using IssueLog.API.Data;
using System.Threading.Tasks;
using IssueLog.API.Dtos;
using IssueLog.API.Models;
using System;
namespace IssueLog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _repo;
        public ProjectController(IProjectRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetProject()
        {
            var projects = await _repo.GetProjects();
            if (projects == null)
                return BadRequest("Fail to Get Project List");
            return Ok(projects);
        }
        [HttpGet("getprojectcomplexity/{projectId}")]
        public async Task<IActionResult> GetProjectComplexity(string projectId)
        {
            var proj = await _repo.GetProjectComplexity(projectId);
            if (proj == null)
            {
                return BadRequest();
            }
            return Ok(proj);
        }

        [HttpGet("getsubprojects")]
        public async Task<IActionResult> GetSubProjects()
        {
            var subProjects = await _repo.GetSubProjects();
            if (subProjects == null)
            {
                return BadRequest("Failure to Get Sub Project List");
            }
            return Ok(subProjects);
        }

        [HttpPost("updateSubProjectStatus")]
        public async Task<IActionResult> UpdateSubProjectStatus(SubProject sub)
        {
            var subProject = await _repo.UpdateSubProjectStatus(sub);
            if (subProject == null)
            {
                return BadRequest("Fail To Update Project Status");
            }
            return Ok(subProject);
        }

        [HttpPost("updateprojectcomplexity")]
        public async Task<IActionResult> UpdateProjectComplexity(ProjectComplexityDto project)
        {
            var success = await _repo.UpdateProjectComplexity(project);
            if (success)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("updateprojectschedule")]
        public async Task<IActionResult> UpdateProjectSchedule(ProjectSchedule projSch)
        {
            if(projSch == null || projSch.Id == null){
                return BadRequest("Empty Input");
            }

            var projSchDb = await _repo.UpdateProjectSchedule(projSch);
            if (projSchDb == null)
            {
                return Ok("New project added in DB " + projSch.Id);
            }
            return Ok(projSchDb);
        }

        [HttpGet("isvalidemployee")]
        public async Task<IActionResult> IsValidEmployee()
        {   
            
            var employeeList = await _repo.IsValidEmployee();
            if(employeeList == null)
            {
                return BadRequest("Can not get employee list from DB.");
            }

            return Ok(employeeList);
        }

        [HttpPost("updateprojectstatus")]
        public async Task<IActionResult> UpdateProjectStatus(TbProject proj)
        {
            var projDb = await _repo.UpdateProjectStatus(proj);
            if (projDb == null)
            {
                return BadRequest("Project number not in DB " + proj.ProjectId);
            }
            return Ok(projDb);
        }

        [HttpPost("updateactionowner")]
        public async Task<IActionResult> UpdateActionOwners(ActionOwner owner)
        {
            var ownerDb = await _repo.UpdateActionOwners(owner);
            if (ownerDb == null)
            {
                return Ok("New Action Owner added in DB " + owner.EmployeeId);
            }
            return Ok(owner);
        }

        
    }
}