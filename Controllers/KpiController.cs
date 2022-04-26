using System.Threading.Tasks;
using IssueLog.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace IssueLog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KpiController : ControllerBase
    {
        private readonly IKpiRepo _repo;

        public KpiController(IKpiRepo repo)
        {
            _repo = repo;

        }
        [HttpGet("GetOtdKpi")]
        public async Task<IActionResult> GetOtdKpi()
        {
            var list = await _repo.GetProjectOtdKpiDtos();
            return Ok(list);
        }
        [HttpGet("GetIssueMissingPartResponse")]
        public async Task<IActionResult> GetIssueMissingPartResponse(){
           var result = await _repo.GetIssueMissingPartResponseReport();
           return Ok(result);
        }

        [HttpGet("GetPartReleaseOtdLR")]
        public async Task<IActionResult> GetPartReleaseOtdLR(){
            var result = await _repo.GetPartReleaseOtoLR();
            if(result==null){
                return BadRequest("Failed to get the report");
            }
            return Ok(result);
        }
        
        [HttpGet("GetPartReleaseOtd")]
        public async Task<IActionResult> GetPartReleaseOtd(){
            var result = await _repo.GetPartReleaseOto();
            if(result==null){
                return BadRequest("Failed to get the report");
            }
            return Ok(result);
        }

    }
}