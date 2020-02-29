using System;
using System.Linq;

namespace DelegationPokerApi.Domain
{
    public class DelegationSession
    {
        private readonly IUserRepository userRepository;
        private readonly IItemRepository itemRepository;

        public DelegationSession(IUserRepository userRepository, IItemRepository itemRepository)
        {
            this.userRepository = userRepository;
            this.itemRepository = itemRepository;
        }

        public SessionData GetData()
        {
            return new SessionData
            {
                Users = userRepository.GetAllUsers(),
                Items = itemRepository.GetAllItems()
            };
        }

        public Guid RegisterUser(string name, UserType type)
        {
            return userRepository.AddUser(name, type);
        }

        public Guid AddItem(string description)
        {
            return itemRepository.AddItem(description);
        }

        public void Vote(Guid itemId, Guid userId, int vote)
        {
            var totalVotes = itemRepository.RegisterVote(itemId, userId, vote);
            if (totalVotes == userRepository.GetAllUsers().Count)
            {
                itemRepository.SetVisibility(itemId, true);
            }
        }

        public void MakeVisible(Guid itemId, Guid userId)
        {
            var user = userRepository.GetAllUsers().FirstOrDefault(x => x.UserId == userId);
            if (user != null && user.Type == UserType.Admin)
            {
                itemRepository.SetVisibility(itemId, true);
            }
        }
    }
}