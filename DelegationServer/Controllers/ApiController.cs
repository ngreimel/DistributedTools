using DelegationServer.Domain;
using DelegationServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DelegationServer.Controllers
{
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly IDelegationSession delegationSession;

        public ApiController(IDelegationSession delegationSession)
        {
            this.delegationSession = delegationSession;
        }

        [HttpGet("state")]
        public IActionResult GetState()
        {
            return Ok(delegationSession.GetData());
        }

        [HttpPost("register")]
        public IActionResult RegisterVoter([FromBody] UserRegistrationRequest request)
        {
            delegationSession.RegisterUser(request.Name, UserType.Voter);
            return Ok();
        }
        
        [HttpPost("register-admin")]
        public IActionResult RegisterAdmin([FromBody] UserRegistrationRequest request)
        {
            delegationSession.RegisterUser(request.Name, UserType.Admin);
            return Ok();
        }
        
        [HttpPost("add-item")]
        public IActionResult AddItem([FromBody] AddItemRequest request)
        {
            delegationSession.AddItem(request.Description);
            return Ok();
        }
        
        [HttpPost("set-current-item")]
        public IActionResult SetCurrentItem([FromBody] SetCurrentItemRequest request)
        {
            delegationSession.SetCurrentItem(request.ItemId, request.UserId);
            return Ok();
        }
        
        [HttpPost("vote")]
        public IActionResult Vote([FromBody] VoteRequest request)
        {
            delegationSession.Vote(request.ItemId, request.UserId, request.Vote);
            return Ok();
        }
        
        [HttpPost("make-visible")]
        public IActionResult MakeVisible([FromBody] MakeVisibleRequest request)
        {
            delegationSession.MakeVisible(request.ItemId, request.UserId);
            return Ok();
        }
    }
}