using System;
using System.Collections.Generic;

namespace DistributedToolsServer.Domain
{
    public class VotingSessionData
    {
        public List<UserWithType> Users { get; set; }
        public Dictionary<Guid, ThumbVote> ThumbVotes { get; set; }
        public string Prompt { get; set; }
        public bool VotesVisible { get; set; }
        public VoteType VoteType { get; set; }
        public Dictionary<Guid, FistToFiveVote> FistToFiveVotes { get; set; }
    }
}
