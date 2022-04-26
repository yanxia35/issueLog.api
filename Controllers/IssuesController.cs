using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IssueLog.API.Data;
using IssueLog.API.Dtos;
using IssueLog.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IssueLog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssuesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IIssueRepository _issueRepo;
        public IssuesController(DataContext context, IIssueRepository issueRepo)
        {
            _issueRepo = issueRepo;
            _context = context;

        }
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            var values = await _context.Issues.Include(x => x.RootCauseProcess)
            .Include(x => x.Originator)
            .Include(x => x.IssueOwner)
            .Include(x => x.IssueActions).ThenInclude(x => x.CreatedBy)
            .Include(x => x.IssueActions).ThenInclude(x => x.Responsible).OrderByDescending(x => x.IssueNo)
            .ToListAsync();

            return Ok(values);
        }

        [HttpPost("addissue")]
        public async Task<IActionResult> AddNewIssue(IssueForAddDto issueForAddDto)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<IssueForAddDto, Issue>());
            var mapper = config.CreateMapper();
            var newIssue = mapper.Map<Issue>(issueForAddDto);
            var issueSaved = await _issueRepo.AddIssue(newIssue);

            return Ok(issueSaved);
        }
        [HttpPost("addissuefromold")]
        public async Task<IActionResult> AddOldIssue(Issue issue)
        {
            var issueSaved = await _issueRepo.AddIssueFromOldDb(issue);

            return Ok(issueSaved);
        }

        [HttpGet("{issueNo}")]
        public async Task<IActionResult> GetIssue(string issueNo)
        {
            var issue = await _issueRepo.GetIssue(issueNo);
            if(issue == null){
                return NotFound("Issue not found");
            }else{
                return Ok(issue);
            }
            
        }
        [HttpGet("getissuebyid/{id}")]
        public async Task<IActionResult> GetIssueById(int id)
        {
            var issue = await _issueRepo.GetIssueById(id);
            return Ok(issue);
        }

        [HttpPost("issueeditsave")]
        public async Task<IActionResult> SaveEdittedIssue(Issue modifiedIssue)
        {
            var issue = await _issueRepo.SaveEdittedIssue(modifiedIssue);
            return Ok(issue);
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search(SearchParaDto searchValues)
        {
            var issues = await _issueRepo.Search(searchValues);
            return Ok(issues);
        }
        [HttpPost("GetIssuesByMonth")]
        public async Task<IActionResult> GetIssuesByMonth([FromBody] int month)
        {
            var issues = await _issueRepo.GetIssuesByMonth(month);
            return Ok(issues);
        }
        [HttpGet("updateissueproject")]
        public async Task<IActionResult> UpdateIssueProject()
        {
            var count = await _issueRepo.UpdateIssueProject();
            return Ok(count);
        }
        [HttpPost("linksubprojects")]
        public async Task<IActionResult> LinkSubProjects(LinkSubProjectsDto link)
        {
            var issue = await _issueRepo.LinkSubProjects(link);
            if (issue == null)
                return BadRequest("Failed to Link the Sub Projects");
            return Ok(issue);
        }

        [HttpPost("linkcustomproject")]
        public async Task<IActionResult> LinkCustomProject(LinkCustomProjectDto req)
        {
            var issue = await _issueRepo.LinkCustomProject(req);
            if (issue == null)
                return BadRequest("Failed to Link the Sub Projects");
            return Ok(issue);
        }
        
        [HttpPost("updatesubprojectstatus")]
        public async Task<IActionResult> UpdateSubProjectStatus(SubProjectStatus sub)
        {
            var subToReturn = await _issueRepo.UpdateSubProjectStatus(sub);
            if (subToReturn == null)
            {
                return BadRequest("Failed to Update Sub Project Status");
            }
            return Ok(subToReturn);
        }
        [HttpPost("deletesubproject")]
        public async Task<IActionResult> DeleteSubProject(LinkCustomProjectDto req)
        {
            var issue = await _issueRepo.DeleteSubProject(req);
            if (issue == null)
            {
                return BadRequest("Failed to delete the sub project!");
            }
            return Ok(issue);
        }
        
        [HttpGet("IsMatCompleted/{issueNo}")]
        public async Task<IActionResult> IsMatCompleted(string issueNo)
        {
            var result = await _issueRepo.IsMatCompleted(issueNo);
            return Ok(result);
        }

    }
}