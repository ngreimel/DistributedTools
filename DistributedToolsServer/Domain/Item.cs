using System;
using System.Collections.Concurrent;

namespace DistributedToolsServer.Domain
{
    public class Item {
        public Guid ItemId { get; set; }
        public string Description { get; set; }
        public ConcurrentDictionary<Guid, int> Votes = new ConcurrentDictionary<Guid, int>();
        public bool IsVisible { get; set; }
    }
}