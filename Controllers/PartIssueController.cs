using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IssueLog.API.Data;
using IssueLog.API.Dtos;
using IssueLog.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IssueLog.API.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class PartIssueController : ControllerBase {
        private readonly IPartIssueRepository _repo;
        public PartIssueController (IPartIssueRepository repo) {
            _repo = repo;
        }

        [HttpPost ("flag")]
        public async Task<IActionResult> FlagPartIssue (PartToFlag part) {
            var partIssue = await _repo.FlagPart (part);
            if (partIssue == null)
                return BadRequest ("Failure to Flag the part");
            return Ok (partIssue);
        }

        [HttpPost ("resolveFlag")]
        public async Task<IActionResult> ResolvePartIssue (PartIssue part) {
            var partIssue = await _repo.ResolveFlag (part);
            if (partIssue == null)
                return BadRequest ("Failure to resolve the flag!");
            return Ok (partIssue);
        }
        [HttpPost ("reopen")]
        public async Task<IActionResult> ReopenPartIssue (PartIssue part) {
            var partIssue = await _repo.ReopenFlag (part);
            if (partIssue == null)
                return BadRequest ("Failure to resolve the flag!");
            return Ok (partIssue);
        }

        [HttpPost ("deleteFlag")]
        public async Task<IActionResult> DeletePartIssue (PartIssue part) {
            var partIssue = await _repo.DeleteFlag (part);
            if (partIssue == 0)
                return BadRequest ("Failure to delete the flag!");
            return Ok (partIssue);
        }

        [HttpGet("getPartIssues/{partNo}")]
        public async Task<IActionResult> GetPartIssues(string partNo){
            var partIssues = await _repo.GetPartIssuesByPartNo(partNo);
            return Ok(partIssues);
        }

    }
}