using System;

namespace DistributedToolsServer.Models
{
    public class MakeVisibleRequest
    {
        public Guid ItemId { get; set; }
        public Guid UserId { get; set; }
    }
}