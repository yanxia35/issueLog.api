using Microsoft.AspNetCore.Mvc;
using IssueLog.API.Data;
using System.Threading.Tasks;
using IssueLog.API.Dtos;

namespace IssueLog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository _repo;

        public ReportController(IReportRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("getreportprojectissuecount")]
        public async Task<IActionResult> GetReportProjectIssueCount(){
          var report = await _repo.GetReportProjectIssueCount();
          if(report == null)
            return BadRequest("Failed to get project issue count report");
          return Ok(report);
        }

        [HttpGet("getreportprojectissuecountbyprojectno/{projectNo}")]
        public async Task<IActionResult> GetReportProjectIssueCountByProjectId(string projectNo){
          var report = await _repo.GetReportProjectIssueCountByProjectNo(projectNo);
          if(report == null)
            return BadRequest("Failed to get project issue count report");
          return Ok(report);
        }

        [HttpGet("getprojectreportedhours")]
        public async Task<IActionResult> GetProjectReportedHours(){
          var projectHours = await _repo.GetProjectHours();
          if(projectHours == null)
            return BadRequest();
          return Ok(projectHours);
        }

        
        [HttpGet("getprojectreportedhoursbyprojectno/{projectNo}")]
        public async Task<IActionResult> GetProjectReportedHoursByProjectNo(string projectNo){
          var projectHours = await _repo.GetProjectHoursByProjectNo(projectNo);
          if(projectHours == null)
            return BadRequest();
          return Ok(projectHours);
        }
        [HttpGet("GetProjectReportedHoursByProjectNoByYears/{projectNo}:{year}")]
        public async Task<IActionResult> GetProjectReportedHoursByProjectNoByYears(string projectNo,int year){
          var projectHours = await _repo.GetProjectReportedHoursByProjectNoByYears(projectNo,year);
          if(projectHours == null)
            return BadRequest();
          return Ok(projectHours);
        }
        // added OTD report as part of this report make my life easier
        [HttpGet("sendReportActionPastDueDate")]
        public async Task<IActionResult> SendReportActionPastDueDate(){
          await _repo.CheckMissingDeliverable();
          await _repo.SendOTDReport();
          var result = await _repo.SendReportActionPastDueDate();
          return Ok(result);
        }

        [HttpGet("sendReportMissingPart")]
        public async Task<IActionResult> SendReportMissingPart(){
          var result = await _repo.SendReportIssueMissingParts();
          return Ok(result);
        }
        [HttpGet("GetIssuesWithoutEcTrackerActionReport")]
        public async Task<IActionResult> GetIssuesWithoutEcTrackerActionReport(){
          var result = await _repo.GetIssuesWithoutEcTrackerActionReport();
          return Ok(result);
        }
        [HttpPost("getEcEta")]
        public async Task<IActionResult> GetEcEta(EcEtaDto ecEtaDto){
            var result = await _repo.GetEcEta(ecEtaDto);
            if (result == null){
              return BadRequest("Not Found");
            }
  
            return Ok(result);
        }
    }
}