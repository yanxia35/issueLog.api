using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IssueLog.API.Data;
using IssueLog.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace IssueLog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EcPartController : ControllerBase
    {
        private readonly IEcPartRepository _repo;
        private readonly IMbomRepo _mbomRepo;

        public EcPartController(IEcPartRepository repo, IMbomRepo mbomRepo)
        {
            _mbomRepo = mbomRepo;
            _repo = repo;
        }
        // GET api/values
        [HttpPost("addpart")]
        public async Task<IActionResult> AddPart(List<EcPartToAddDto> ecParts)
        {
            var success = await _repo.AddPart(ecParts);
            if (!success)
            {
                return BadRequest("Wrong issue numbers");
            }
            return Ok(success);
        }

        [HttpGet("deleteall")]
        public async Task<IActionResult> DeleteAll()
        {
            var success = await _repo.DeleteAll();
            return Ok(success);
        }
        [HttpGet("deleteEcPartsByProjectNo/{projectNo}")]
        public async Task<IActionResult> DeleteEcPartsByProjectNo(string projectNo)
        {
            var success = await _repo.DeleteEcPartsByProject(projectNo);
            return Ok(success);
        }

        // GET api/values/5
        [HttpGet("deleteMbom/{projectNo}")]
        public async Task<IActionResult> DeleteMbom(string projectNo){
          var task = await _mbomRepo.deleteMbomByProject(projectNo);
          return Ok(task);
        }

        [HttpPost("addMbomPart")]
        public async Task<IActionResult> AddMbomPart(List<MbomPart> parts){
          var task = await _mbomRepo.AddPart(parts);
          return Ok(task);
        } 
        [HttpGet("deleteAllMbomParts")]
        public async Task<IActionResult> DeleteAllMbomParts(){
          var task = await _mbomRepo.DeleteAll();
          return Ok(task);
        } 

    }
}
