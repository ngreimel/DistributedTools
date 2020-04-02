using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace DistributedToolsServer.Domain
{
    public interface IRoomSessionRepository
    {
        void CreateDecisionDelegationSession(string roomCode);
        IDecisionDelegationSession GetDecisionDelegationSession(string roomCode);
        void CreateVotingSession(string roomCode);
        IVotingSession GetVotingSession(string roomCode);
    }

    public class RoomSessionRepository : IRoomSessionRepository
    {
        private readonly IServiceProvider serviceProvider;
        private ConcurrentDictionary<string, IDecisionDelegationSession> decisionDelegationSessions = new ConcurrentDictionary<string, IDecisionDelegationSession>();
        private ConcurrentDictionary<string, IVotingSession> votingSessions = new ConcurrentDictionary<string, IVotingSession>();

        public RoomSessionRepository(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void CreateDecisionDelegationSession(string roomCode)
        {
            decisionDelegationSessions.TryAdd(roomCode, serviceProvider.GetService<IDecisionDelegationSession>());
        }

        public IDecisionDelegationSession GetDecisionDelegationSession(string roomCode)
        {
            return decisionDelegationSessions.GetValueOrDefault(roomCode);
        }

        public void CreateVotingSession(string roomCode)
        {
            votingSessions.TryAdd(roomCode, serviceProvider.GetService<IVotingSession>());
        }

        public IVotingSession GetVotingSession(string roomCode)
        {
            return votingSessions.GetValueOrDefault(roomCode);
        }
    }
}
