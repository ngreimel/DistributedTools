using System;
using System.Collections.Generic;

namespace DistributedToolsServer.Domain
{
    public class SessionData
    {
        public List<User> Users { get; set; }
        public List<Item> Items { get; set; }
        public Guid CurrentItemId { get; set; }
    }
}