using System.Linq;
using DistributedToolsServer.Domain;
using DistributedToolsServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DistributedToolsServer.Controllers
{
    [Route("decision-delegation/{roomCode}")]
    public class DecisionDelegationPokerController : Controller
    {
        private readonly IRoomSessionRepository roomSessionRepository;

        public DecisionDelegationPokerController(IRoomSessionRepository roomSessionRepository)
        {
            this.roomSessionRepository = roomSessionRepository;
        }

        [Route("")]
        public IActionResult Index(string roomCode)
        {
            return View("Index", roomCode);
        }

        private IDecisionDelegationSession getSession(string roomCode)
        {
            return roomSessionRepository.GetDecisionDelegationSession(roomCode);
        }

        [HttpGet("state")]
        public IActionResult GetState(string roomCode)
        {
            var session = getSession(roomCode);
            if (session == null)
            {
                roomSessionRepository.CreateSession(roomCode);
                session = getSession(roomCode);
            }
            var data = session.GetData();
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
        public IActionResult RegisterVoter(string roomCode, [FromBody] UserRegistrationRequest request)
        {
            var userId = getSession(roomCode).RegisterUser(request.Name, UserType.Voter);
            return Ok(new { userId });
        }

        [HttpPost("register-admin")]
        public IActionResult RegisterAdmin(string roomCode, [FromBody] UserRegistrationRequest request)
        {
            var userId = getSession(roomCode).RegisterUser(request.Name, UserType.Admin);
            return Ok(new { userId });
        }

        [HttpPost("add-item")]
        public IActionResult AddItem(string roomCode, [FromBody] AddItemRequest request)
        {
            getSession(roomCode).AddItem(request.Description);
            return Ok(new {});
        }

        [HttpPost("set-current-item")]
        public IActionResult SetCurrentItem(string roomCode, [FromBody] SetCurrentItemRequest request)
        {
            getSession(roomCode).SetCurrentItem(request.ItemId, request.UserId);
            return Ok(new {});
        }

        [HttpPost("vote")]
        public IActionResult Vote(string roomCode, [FromBody] VoteRequest request)
        {
            getSession(roomCode).Vote(request.ItemId, request.UserId, request.Vote);
            return Ok(new {});
        }

        [HttpPost("make-visible")]
        public IActionResult MakeVisible(string roomCode, [FromBody] MakeVisibleRequest request)
        {
            getSession(roomCode).MakeVisible(request.ItemId, request.UserId);
            return Ok(new {});
        }
    }
}
