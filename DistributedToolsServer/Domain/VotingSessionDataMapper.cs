using System.Linq;

namespace DistributedToolsServer.Domain
{
    public interface IVotingSessionDataMapper
    {
        object Map(VotingSessionData data);
    }

    public class VotingSessionDataMapper : IVotingSessionDataMapper
    {
        public object Map(VotingSessionData data)
        {
            return new
            {
                users = data.Users,
                prompt = data.Prompt,
                votesVisible = data.VotesVisible,
                voteType = data.VoteType,
                thumbVotes = data.ThumbVotes.Select(x => new { userId = x.Key, vote = x.Value }).ToList(),
                fistToFiveVotes = data.FistToFiveVotes.Select(x => new { userId = x.Key, vote = x.Value }).ToList()
            };
        }
    }
}
