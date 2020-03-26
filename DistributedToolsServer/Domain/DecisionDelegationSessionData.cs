using System;
using System.Collections.Generic;

namespace DistributedToolsServer.Domain
{
    public class DecisionDelegationSessionData
    {
        public List<User> Users { get; set; }
        public List<DecisionDelegationItem> Items { get; set; }
        public Guid CurrentItemId { get; set; }
    }
}