using System.Threading.Tasks;
using IssueLog.API.Data;
using IssueLog.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace IssueLog.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class FailureModeController : ControllerBase
  {
    private readonly IFailureModeRepository _repo;

    public FailureModeController(IFailureModeRepository repo)
    {
      _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetFailureMode(){
      var failureMode = await _repo.GetFailureMode();
      return Ok(failureMode);
    }

    [HttpPost("addFailureMode")]
    public async Task<IActionResult> AddFailureMode(FailureMode failureMode){
        var failureModeAdded = await _repo.AddFailureMode(failureMode);
        return Ok(failureMode);
    }
  }
}