using DistributedToolsServer.Domain;
using DistributedToolsServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DistributedToolsServer.Controllers
{
    [Route("decision-delegation")]
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

        [Route("{roomCode}")]
        public IActionResult Index(string roomCode)
        {
            return View("Index", new RoomCodeAndUser(roomCode, currentUserAccessor.GetCurrentUser()?.UserId));
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
            roomSessionRepository.GetDecisionDelegationSession(roomCode).AddUser(user, UserType.Admin);
            return Redirect($"/decision-delegation/{roomCode}");
        }

        [HttpPost("join-room")]
        public IActionResult JoinRoom([FromForm] string roomCode)
        {
            var user = currentUserAccessor.GetCurrentUser();
            var session = roomSessionRepository.GetDecisionDelegationSession(roomCode);
            if (user == null || session == null)
            {
                return BadRequest();
            }

            session.AddUser(user, UserType.Voter);
            return Redirect($"/decision-delegation/{roomCode}");
        }
    }
}
