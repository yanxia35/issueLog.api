using System.Threading.Tasks;
using IssueLog.API.Data;
using IssueLog.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace IssueLog.API.Controllers {
  [Route ("api/[controller]")]
  [ApiController]
  public class ProcessController : ControllerBase {
    private readonly IProcessRepository _repo;
    public ProcessController (IProcessRepository repo) {
      _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetProcesses () {
      var process = await _repo.GetProcesses ();
      return Ok (process);
    }

    [HttpPost ("addProcess")]
    public async Task<IActionResult> AddProcess (Process process) {
      var processAdded = await _repo.AddProcess (process);
      return Ok (processAdded);
    }
  }
}