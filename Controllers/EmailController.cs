using System.Threading.Tasks;
using IssueLog.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace IssueLog.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class EmailController : ControllerBase
  {
    private readonly IIssueRepository _issueRepo;
    public EmailController(IIssueRepository issueRepo)
    {
      _issueRepo = issueRepo;

    }
    [HttpGet("newissuereport")]
    public async Task<IActionResult> SendNewIssueReport()
    {
      await _issueRepo.GetNewIssueReport();
      return Ok("ok");
    }
  }
}