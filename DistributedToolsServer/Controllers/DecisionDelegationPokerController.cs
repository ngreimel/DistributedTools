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
        private readonly ICurrentUserAccessor currentUserAccessor;
        private readonly IRoomCodeGenerator roomCodeGenerator;

        public DecisionDelegationPokerController(IRoomSessionRepository roomSessionRepository, ICurrentUserAccessor currentUserAccessor, IRoomCodeGenerator roomCodeGenerator)
        {
            this.roomSessionRepository = roomSessionRepository;
            this.currentUserAccessor = currentUserAccessor;
            this.roomCodeGenerator = roomCodeGenerator;
        }

        [Route("")]
        public IActionResult Index(string roomCode)
        {
            return View("Index", new RoomCodeAndUser(roomCode, currentUserAccessor.GetCurrentUser()?.UserId));
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

        [HttpPost("create-room")]
        public IActionResult CreateRoom()
        {
            var user = currentUserAccessor.GetCurrentUser();
            if (user == null)
            {
                return BadRequest();
            }

            var roomCode = roomCodeGenerator.Generate();
            roomSessionRepository.CreateSession(roomCode);
            getSession(roomCode).AddUser(user, UserType.Admin);
            return Redirect($"/decision-delegation/{roomCode}");
        }

        [HttpPost("join-room")]
        public IActionResult JoinRoom([FromForm] string roomCode)
        {
            var user = currentUserAccessor.GetCurrentUser();
            var session = getSession(roomCode);
            if (user == null || session == null)
            {
                return BadRequest();
            }

            session.AddUser(user, UserType.Voter);
            return Redirect($"/decision-delegation/{roomCode}");
        }

        [HttpPost("join")]
        public IActionResult RegisterVoter(string roomCode)
        {
            var user = currentUserAccessor.GetCurrentUser();
            if (user == null)
            {
                return BadRequest();
            }
            getSession(roomCode).AddUser(user, UserType.Voter);
            return Ok();
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
