using System.Threading.Tasks;
using IssueLog.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace IssueLog.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class EmployeeController : ControllerBase
  {
    private readonly IEmployeeRepository _repo;
    public EmployeeController(IEmployeeRepository repo)
    {
      _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var employees = await _repo.GetEmployees();
        return Ok(employees);
    }
    [HttpGet("getall")]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _repo.GetAllEmployees();
        return Ok(employees);
    }
  }
}