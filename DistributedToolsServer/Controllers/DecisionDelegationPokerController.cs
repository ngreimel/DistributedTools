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

        public DecisionDelegationPokerController(IRoomSessionRepository roomSessionRepository,
            ICurrentUserAccessor currentUserAccessor,
            IRoomCodeGenerator roomCodeGenerator)
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
    }
}
