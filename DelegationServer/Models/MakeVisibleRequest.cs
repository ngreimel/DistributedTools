using System;

namespace DelegationServer.Models
{
    public class MakeVisibleRequest
    {
        public Guid ItemId { get; set; }
        public Guid UserId { get; set; }
    }
}