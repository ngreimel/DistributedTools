using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace DistributedToolsServer.Domain
{
    public interface IRoomSessionRepository
    {
        void CreateSession(string roomCode);
        IDecisionDelegationSession GetDecisionDelegationSession(string roomCode);
    }

    public class RoomSessionRepository : IRoomSessionRepository
    {
        private readonly IServiceProvider serviceProvider;
        private ConcurrentDictionary<string, IDecisionDelegationSession> decisionDelegationSessions = new ConcurrentDictionary<string, IDecisionDelegationSession>();

        public RoomSessionRepository(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        
        public void CreateSession(string roomCode)
        {
            decisionDelegationSessions.TryAdd(roomCode, serviceProvider.GetService<IDecisionDelegationSession>());
        }

        public IDecisionDelegationSession GetDecisionDelegationSession(string roomCode)
        {
            return decisionDelegationSessions.GetValueOrDefault(roomCode);
        }
    }
}