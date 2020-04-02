using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DistributedToolsServer.Domain
{
    public class VotingHub : Hub
    {
        private readonly IRoomSessionRepository roomSessionRepository;
        private readonly IVotingSessionDataMapper votingSessionDataMapper;

        public VotingHub(IRoomSessionRepository roomSessionRepository, IVotingSessionDataMapper votingSessionDataMapper)
        {
            this.roomSessionRepository = roomSessionRepository;
            this.votingSessionDataMapper = votingSessionDataMapper;
        }

        public async Task ConnectToRoom(string roomCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            var session = roomSessionRepository.GetVotingSession(roomCode);
            if (session != null)
            {
                var data = votingSessionDataMapper.Map(session.GetData());
                await Clients.Caller.SendAsync("StateUpdate", data);
            }
        }
    }
}
