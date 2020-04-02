using System;
using System.Collections.Concurrent;
using System.Linq;

namespace DistributedToolsServer.Domain
{
    public class VotingSession
    {
        private readonly IUserGroup userGroup;
        private readonly ConcurrentDictionary<Guid, ThumbVote> thumbVotes = new ConcurrentDictionary<Guid, ThumbVote>();
        private readonly ConcurrentDictionary<Guid, FistToFiveVote> fistToFiveVotes = new ConcurrentDictionary<Guid, FistToFiveVote>();
        private string prompt = "Do you concur?";
        private bool votesVisible = false;
        private VoteType voteType = VoteType.Thumbs;

        public VotingSession(IUserGroup userGroup)
        {
            this.userGroup = userGroup;
        }

        public VotingSessionData GetData()
        {
            return new VotingSessionData
            {
                Users = userGroup.GetAllUsers(),
                ThumbVotes = thumbVotes.ToDictionary(x => x.Key, x => x.Value),
                FistToFiveVotes = fistToFiveVotes.ToDictionary(x => x.Key, x => x.Value),
                Prompt = prompt,
                VotesVisible = votesVisible,
                VoteType = voteType
            };
        }

        public void AddUser(User user, UserType type)
        {
            userGroup.AddUser(user, type);
        }

        public void ThumbVote(Guid userId, ThumbVote vote)
        {
            var users = userGroup.GetAllUsers();
            if (users.Any(x => x.UserId == userId))
            {
                thumbVotes[userId] = vote;
                if (voteType == VoteType.Thumbs && users.All(x => thumbVotes.ContainsKey(x.UserId)))
                {
                    votesVisible = true;
                }
            }
        }

        public void SetPrompt(Guid userId, string newPrompt)
        {
            if (IsAdmin(userId))
            {
                prompt = newPrompt;
                votesVisible = false;
                thumbVotes.Clear();
            }
        }

        public void MakeVotesVisible(Guid userId)
        {
            if (IsAdmin(userId))
            {
                votesVisible = true;
            }
        }

        private bool IsAdmin(Guid userId)
        {
            return userGroup.GetAllUsers().Any(x => x.UserId == userId && x.Type == UserType.Admin);
        }

        public void SetVotingType(Guid userId, VoteType voteType)
        {
            if (IsAdmin(userId))
            {
                this.voteType = voteType;
            }
        }

        public void FistToFiveVote(Guid userId, FistToFiveVote vote)
        {
            var users = userGroup.GetAllUsers();
            if (users.Any(x => x.UserId == userId))
            {
                fistToFiveVotes[userId] = vote;
                if (voteType == VoteType.FistToFive && users.All(x => fistToFiveVotes.ContainsKey(x.UserId)))
                {
                    votesVisible = true;
                }
            }
        }
    }
}
