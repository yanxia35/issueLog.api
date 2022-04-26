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
    public class SubscriberController : ControllerBase
    {
        private readonly ISubscriberRepository _repo;

        public SubscriberController(ISubscriberRepository repo)
      {
            _repo = repo;
        }
        // GET api/values
        [HttpPost("addsubscriber")]
        public async Task<IActionResult> AddSubscriber(Subscriber subscriber)
        {
          var subToReturn = await _repo.AddSubscriber(subscriber);
          if(subToReturn ==null)
            return BadRequest("Failed to subscribe!");
          return Ok(subToReturn); 
        }
                // GET api/values
        [HttpPost("deletesubscriber")]
        public async Task<IActionResult> DeleteSubscriber(Subscriber subscriber)
        {
          var subToReturn = await _repo.DeleteSubscriber(subscriber);
          if(subToReturn ==0)
            return BadRequest("Failed to unsubscribe!");
          return Ok(subToReturn); 
        }

    }
}
