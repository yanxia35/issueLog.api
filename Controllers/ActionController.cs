using System.Threading.Tasks;
using IssueLog.API.Data;
using IssueLog.API.Models;
using Microsoft.AspNetCore.Mvc;
using IssueLog.API.Dtos;
using AutoMapper;

namespace IssueLog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionController : ControllerBase
    {
        private readonly IActionRepository _actionRepo;
        public ActionController(IActionRepository actionRepo)
        {
            _actionRepo = actionRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllActions(){
            var actions = await _actionRepo.GetAllActions();
            return Ok(actions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAction(int id)
        {
            var action = await _actionRepo.GetAction(id);
            return Ok(action);
        }

        [HttpPost("addaction")]
        public async Task<IActionResult> AddNewAction(ActionForAddDto actionForAdd)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<ActionForAddDto, IssueAction>());
            var mapper = config.CreateMapper();
            var newAction = mapper.Map<IssueAction>(actionForAdd);
            var issueSaved = await _actionRepo.AddAction(newAction);
            return Ok(issueSaved);
        }
        [HttpPost("uploadAction")]
        public async Task<IActionResult> UploadAction(IssueAction newAction)
        {
            var issueSaved = await _actionRepo.UploadAction(newAction);
            return Ok(issueSaved);
        }
        [HttpPost("deleteaction")]
        public async Task<IActionResult> DeleteAction(IssueAction actionDelete)
        {
            var issueSaved = await _actionRepo.DeleteAction(actionDelete);
            if(issueSaved==0){
                return BadRequest("Failed to Delete the Action!");
            }
            return Ok(issueSaved);
        }
        [HttpPost("getActionsByMonth")]
        public async Task<IActionResult> GetActionsByMonth([FromBody]int month)
        {
            var actions = await _actionRepo.GetActionsByMonth(month);

            return Ok(actions);
        }

        [HttpPost("saveaction")]
        public async Task<IActionResult> SaveAction(IssueAction savedAction)
        {
            var action = await _actionRepo.SaveAction(savedAction);
            return Ok(action);
        }

        [HttpPost("getactionowner")]
        public async Task<IActionResult> GetActionOwner(ActionOwnerCondition condition)
        {
            var actionOwner = await _actionRepo.GetActionOwner(condition);
            if(actionOwner.actionTemplate == "" || actionOwner.message!="success"){
                return NotFound(actionOwner.message);
            }else{
                return Ok(actionOwner);
            }
            
        }

        [HttpPost("getecdrawingowner")]
        public async Task<IActionResult> GetEcDrawingOwner(ActionOwnerCondition condition)
        {
            var actionOwner = await _actionRepo.GetEcDrawingOwner(condition);
            return Ok(actionOwner);
        }
    }
}