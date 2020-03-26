using System;

namespace DistributedToolsServer.Models
{
    public class SetCurrentItemRequest
    {
        public Guid ItemId { get; set; }
        public Guid UserId { get; set; }
    }
}