using System;
using System.Linq;

namespace DistributedToolsServer.Domain
{
    public interface IDelegationSession
    {
        SessionData GetData();
        Guid RegisterUser(string name, UserType type);
        Guid AddItem(string description);
        void Vote(Guid itemId, Guid userId, int vote);
        void MakeVisible(Guid itemId, Guid userId);
        void SetCurrentItem(Guid itemId, Guid userId);
    }

    public class DelegationSession : IDelegationSession
    {
        private readonly IUserGroup userGroup;
        private readonly IItemRepository itemRepository;
        private Guid currentItemId = Guid.Empty;

        public DelegationSession(IUserGroup userGroup, IItemRepository itemRepository)
        {
            this.userGroup = userGroup;
            this.itemRepository = itemRepository;
        }

        public SessionData GetData()
        {
            return new SessionData
            {
                Users = userGroup.GetAllUsers(),
                Items = itemRepository.GetAllItems(),
                CurrentItemId = currentItemId
            };
        }

        public Guid RegisterUser(string name, UserType type)
        {
            return userGroup.AddUser(name, type);
        }

        public Guid AddItem(string description)
        {
            return itemRepository.AddItem(description);
        }

        public void Vote(Guid itemId, Guid userId, int vote)
        {
            var totalVotes = itemRepository.RegisterVote(itemId, userId, vote);
            if (totalVotes == userGroup.GetAllUsers().Count)
            {
                itemRepository.SetVisibility(itemId, true);
            }
        }

        public void MakeVisible(Guid itemId, Guid userId)
        {
            var user = userGroup.GetAllUsers().FirstOrDefault(x => x.UserId == userId);
            if (user != null && user.Type == UserType.Admin)
            {
                itemRepository.SetVisibility(itemId, true);
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