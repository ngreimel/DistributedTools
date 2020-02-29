using System;

namespace DelegationServer.Models
{
    public class SetCurrentItemRequest
    {
        public Guid ItemId { get; set; }
        public Guid UserId { get; set; }
    }
}