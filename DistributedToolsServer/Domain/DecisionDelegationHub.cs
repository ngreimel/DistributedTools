using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DistributedToolsServer.Domain
{
    public class DecisionDelegationHub : Hub
    {
        private readonly IRoomSessionRepository roomSessionRepository;
        private readonly IDecisionDelegationDataMapper decisionDelegationDataMapper;
        private readonly IUserRepository userRepository;

        public DecisionDelegationHub(IRoomSessionRepository roomSessionRepository, IDecisionDelegationDataMapper decisionDelegationDataMapper, IUserRepository userRepository)
        {
            this.roomSessionRepository = roomSessionRepository;
            this.decisionDelegationDataMapper = decisionDelegationDataMapper;
            this.userRepository = userRepository;
        }

        public async Task ConnectToRoom(string roomCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            var session = roomSessionRepository.GetDecisionDelegationSession(roomCode);
            if (session != null)
            {
                var data = decisionDelegationDataMapper.Map(session.GetData());
                await Clients.Caller.SendAsync("StateUpdate", data);
            }
        }

        public async Task Join(string roomCode, Guid userId)
        {
            var session = roomSessionRepository.GetDecisionDelegationSession(roomCode);
            var user = userRepository.GetUser(userId);

            if (session != null && user != null)
            {
                session.AddUser(user, UserType.Voter);
                await PublishState(roomCode, session);
            }
        }

        public async Task AddItem(string roomCode, string description)
        {
            var session = roomSessionRepository.GetDecisionDelegationSession(roomCode);
            if (session != null)
            {
                session.AddItem(description);
                await PublishState(roomCode, session);
            }
        }

        public async Task SetCurrentItem(string roomCode, Guid itemId, Guid userId)
        {
            var session = roomSessionRepository.GetDecisionDelegationSession(roomCode);
            if (session != null)
            {
                session.SetCurrentItem(itemId, userId);
                await PublishState(roomCode, session);
            }
        }

        public async Task Vote(string roomCode, Guid itemId, Guid userId, int vote)
        {
            var session = roomSessionRepository.GetDecisionDelegationSession(roomCode);
            if (session != null)
            {
                session.Vote(itemId, userId, vote);
                await PublishState(roomCode, session);
            }
        }

        public async Task MakeVisible(string roomCode, Guid itemId, Guid userId)
        {
            var session = roomSessionRepository.GetDecisionDelegationSession(roomCode);
            if (session != null)
            {
                session.MakeVisible(itemId, userId);
                await PublishState(roomCode, session);
            }
        }

        private async Task PublishState(string roomCode, IDecisionDelegationSession session)
        {
            var data = decisionDelegationDataMapper.Map(session.GetData());
            await Clients.Group(roomCode).SendAsync("StateUpdate", data);
        }
    }
}
