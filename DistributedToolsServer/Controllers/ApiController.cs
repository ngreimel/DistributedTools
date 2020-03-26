using System.Linq;
using DistributedToolsServer.Domain;
using DistributedToolsServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DistributedToolsServer.Controllers
{
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly IDecisionDelegationSession decisionDelegationSession;

        public ApiController(IDecisionDelegationSession decisionDelegationSession)
        {
            this.decisionDelegationSession = decisionDelegationSession;
        }

        [HttpGet("state")]
        public IActionResult GetState()
        {
            var data = decisionDelegationSession.GetData();
            return Ok(new
            {
                data.Users,
                Items = data.Items.Select(i => new
                {
                    i.ItemId,
                    i.Description,
                    i.IsVisible,
                    Votes = i.Votes
                        .Select(v => new
                        {
                            userId = v.Key,
                            vote = v.Value
                        })
                        .ToList()
                }),
                data.CurrentItemId
            });
        }

        [HttpPost("register")]
        public IActionResult RegisterVoter([FromBody] UserRegistrationRequest request)
        {
            var userId = decisionDelegationSession.RegisterUser(request.Name, UserType.Voter);
            return Ok(new { userId });
        }
        
        [HttpPost("register-admin")]
        public IActionResult RegisterAdmin([FromBody] UserRegistrationRequest request)
        {
            var userId = decisionDelegationSession.RegisterUser(request.Name, UserType.Admin);
            return Ok(new { userId });
        }
        
        [HttpPost("add-item")]
        public IActionResult AddItem([FromBody] AddItemRequest request)
        {
            decisionDelegationSession.AddItem(request.Description);
            return Ok(new {});
        }
        
        [HttpPost("set-current-item")]
        public IActionResult SetCurrentItem([FromBody] SetCurrentItemRequest request)
        {
            decisionDelegationSession.SetCurrentItem(request.ItemId, request.UserId);
            return Ok(new {});
        }
        
        [HttpPost("vote")]
        public IActionResult Vote([FromBody] VoteRequest request)
        {
            decisionDelegationSession.Vote(request.ItemId, request.UserId, request.Vote);
            return Ok(new {});
        }
        
        [HttpPost("make-visible")]
        public IActionResult MakeVisible([FromBody] MakeVisibleRequest request)
        {
            decisionDelegationSession.MakeVisible(request.ItemId, request.UserId);
            return Ok(new {});
        }
    }
}