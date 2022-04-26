using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IssueLog.API.Data;
using IssueLog.API.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace IssueLog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueFileController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IIssueFileRepository _repo;

        public IssueFileController(IWebHostEnvironment env, IIssueFileRepository repo)
        {
            _env = env;
            _repo = repo;
        }
        [HttpPost("addfile")]
        public async Task<IActionResult> AddFile([FromForm] IssueFileToAddDto issueFileToAddDto)
        {

            if (issueFileToAddDto.File == null)
                return BadRequest("No File Was Sent");
            // add file info to the database
            var issueFile = await _repo.AddIssueFile(issueFileToAddDto);
            //save file to the folder
            return Ok(issueFile);
        }
        [HttpGet("getissuefilebyid/{id}")]
        public async Task<IActionResult> GetIssueFileById(int id)
        {
            var issueFiles = await _repo.GetIssueFileById(id);
            if (issueFiles == null)
                return Ok(new List<int>());
            return Ok(issueFiles);
        }
        [HttpGet("deleteissuefile/{id}")]
        public async Task<IActionResult> DeleteIssueFile(int id)
        {
            var success = await _repo.DeleteIssueFile(id);
            if (!success)
            {
                return BadRequest("Failed to delete issue file!");
            }
            return Ok(success);
        }

    }
}