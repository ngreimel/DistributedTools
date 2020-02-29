using System;

namespace DelegationServer.Models
{
    public class VoteRequest
    {
        public Guid ItemId { get; set; }
        public Guid UserId { get; set; }
        public int Vote { get; set; }
    }
}