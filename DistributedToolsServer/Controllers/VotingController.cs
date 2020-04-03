using DistributedToolsServer.Domain;
using DistributedToolsServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DistributedToolsServer.Controllers
{
    [Route("voting")]
    public class VotingController : Controller
    {
        private readonly ICurrentUserAccessor currentUserAccessor;
        private readonly IRoomCodeGenerator roomCodeGenerator;
        private readonly IRoomSessionRepository roomSessionRepository;

        public VotingController(ICurrentUserAccessor currentUserAccessor,
            IRoomCodeGenerator roomCodeGenerator,
            IRoomSessionRepository roomSessionRepository)
        {
            this.roomCodeGenerator = roomCodeGenerator;
            this.roomSessionRepository = roomSessionRepository;
            this.currentUserAccessor = currentUserAccessor;
        }

        [Route("{roomCode}")]
        public IActionResult Index(string roomCode)
        {
            var upperRoomCode = roomCode.ToUpper();
            var session = roomSessionRepository.GetVotingSession(upperRoomCode);
            if (session == null)
            {
                return View("InvalidRoom");
            }

            return View("Index", new RoomCodeAndUser(upperRoomCode, currentUserAccessor.GetCurrentUser()?.UserId));
        }

        [HttpPost("create-room")]
        public IActionResult CreateRoom()
        {
            var user = currentUserAccessor.GetCurrentUser();
            if (user == null)
            {
                return View("NotRegistered");
            }

            var roomCode = roomCodeGenerator.Generate();
            roomSessionRepository.CreateVotingSession(roomCode);
            roomSessionRepository.GetVotingSession(roomCode).AddUser(user, UserType.Admin);
            return Redirect($"/voting/{roomCode}");
        }

        [HttpPost("join-room")]
        public IActionResult JoinRoom([FromForm] string roomCode)
        {
            var upperRoomCode = roomCode.ToUpper();
            var user = currentUserAccessor.GetCurrentUser();
            if (user == null)
            {
                return View("NotRegistered");
            }

            var session = roomSessionRepository.GetVotingSession(upperRoomCode);
            if (session == null)
            {
                return View("InvalidRoom");
            }

            session.AddUser(user, UserType.Voter);
            return Redirect($"/voting/{upperRoomCode}");
        }
    }
}
