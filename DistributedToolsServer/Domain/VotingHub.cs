using System;
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

        public async Task NewVote(string roomCode, Guid userId, VoteType type, string prompt)
        {
            await WithSession(roomCode, session =>
            {
                session.SetVotingType(userId, type);
                session.SetPrompt(userId, prompt);
            });
        }

        public async Task ThumbVote(string roomCode, Guid userId, ThumbVote vote)
        {
            await WithSession(roomCode, session => session.ThumbVote(userId, vote));
        }

        public async Task FistToFiveVote(string roomCode, Guid userId, FistToFiveVote vote)
        {
            await WithSession(roomCode, session => session.FistToFiveVote(userId, vote));
        }

        public async Task MakeVotesVisible(string roomCode, Guid userId)
        {
            await WithSession(roomCode, session => session.MakeVotesVisible(userId));
        }

        private async Task WithSession(string roomCode, Action<IVotingSession> action)
        {
            var session = roomSessionRepository.GetVotingSession(roomCode);
            if (session != null)
            {
                action(session);
                var data = votingSessionDataMapper.Map(session.GetData());
                await Clients.Group(roomCode).SendAsync("StateUpdate", data);
            }
        }
    }
}
