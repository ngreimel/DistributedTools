using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DistributedToolsServer.Domain
{
    public interface IDecisionDelegationItemGroup
    {
        List<DecisionDelegationItem> GetAllItems();
        Guid AddItem(string description);
        void Remove(Guid itemId);
        int RegisterVote(Guid itemId, Guid userId, int vote);
        bool SetVisibility(Guid itemId, bool isVisible);
    }

    public class DecisionDelegationDecisionDelegationItemGroup : IDecisionDelegationItemGroup
    {
        private readonly ConcurrentDictionary<Guid, DecisionDelegationItem> items = new ConcurrentDictionary<Guid, DecisionDelegationItem>();
        
        public List<DecisionDelegationItem> GetAllItems()
        {
            return items.Values.ToList();
        }

        public Guid AddItem(string description)
        {
            var itemId = Guid.NewGuid();
            items[itemId] = new DecisionDelegationItem
            {
                ItemId = itemId,
                Description = description
            };
            return itemId;
        }

        public void Remove(Guid itemId)
        {
            items.TryRemove(itemId, out var _);
        }

        public int RegisterVote(Guid itemId, Guid userId, int vote)
        {
            if (items.ContainsKey(itemId))
            {
                items[itemId].Votes[userId] = vote;
                return items[itemId].Votes.Count;
            }

            return 0;
        }

        public bool SetVisibility(Guid itemId, bool isVisible)
        {
            if (items.ContainsKey(itemId))
            {
                items[itemId].IsVisible = isVisible;
                return isVisible;    
            }

            return false;
        }
    }
}