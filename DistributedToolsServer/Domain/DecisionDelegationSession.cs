using System;
using System.Linq;

namespace DistributedToolsServer.Domain
{
    public interface IDecisionDelegationSession
    {
        DecisionDelegationSessionData GetData();
        Guid RegisterUser(string name, UserType type);
        Guid AddItem(string description);
        void Vote(Guid itemId, Guid userId, int vote);
        void MakeVisible(Guid itemId, Guid userId);
        void SetCurrentItem(Guid itemId, Guid userId);
    }

    public class DecisionDelegationSession : IDecisionDelegationSession
    {
        private readonly IUserGroup userGroup;
        private readonly IDecisionDelegationItemGroup decisionDelegationItemGroup;
        private Guid currentItemId = Guid.Empty;

        public DecisionDelegationSession(IUserGroup userGroup, IDecisionDelegationItemGroup decisionDelegationItemGroup)
        {
            this.userGroup = userGroup;
            this.decisionDelegationItemGroup = decisionDelegationItemGroup;
        }

        public DecisionDelegationSessionData GetData()
        {
            return new DecisionDelegationSessionData
            {
                Users = userGroup.GetAllUsers(),
                Items = decisionDelegationItemGroup.GetAllItems(),
                CurrentItemId = currentItemId
            };
        }

        public Guid RegisterUser(string name, UserType type)
        {
            return userGroup.AddUser(name, type);
        }

        public Guid AddItem(string description)
        {
            return decisionDelegationItemGroup.AddItem(description);
        }

        public void Vote(Guid itemId, Guid userId, int vote)
        {
            var totalVotes = decisionDelegationItemGroup.RegisterVote(itemId, userId, vote);
            if (totalVotes == userGroup.GetAllUsers().Count)
            {
                decisionDelegationItemGroup.SetVisibility(itemId, true);
            }
        }

        public void MakeVisible(Guid itemId, Guid userId)
        {
            var user = userGroup.GetAllUsers().FirstOrDefault(x => x.UserId == userId);
            if (user != null && user.Type == UserType.Admin)
            {
                decisionDelegationItemGroup.SetVisibility(itemId, true);
            }
        }

        public void SetCurrentItem(Guid itemId, Guid userId)
        {
            var user = userGroup.GetAllUsers().FirstOrDefault(x => x.UserId == userId);
            if (user != null && user.Type == UserType.Admin)
            {
                currentItemId = itemId;
            }
        }
    }
}