using System;
using DistributedToolsServer.Domain;
using NUnit.Framework;

namespace DistributedToolsServer.UnitTests.Domain
{
    public class VotingSessionTests
    {
        private VotingSession classUnderTest;

        [SetUp]
        public void SetUp()
        {
            classUnderTest = new VotingSession(new UserGroup());
        }

        [Test]
        public void When_constructing()
        {
            var result = classUnderTest.GetData();

            Assert.That(result.Users.Count, Is.EqualTo(0));
            Assert.That(result.ThumbVotes.Count, Is.EqualTo(0));
            Assert.That(result.Prompt, Is.EqualTo("Do you concur?"));
            Assert.That(result.VotesVisible, Is.False);
            Assert.That(result.VoteType, Is.EqualTo(VoteType.Thumbs));
        }

        [TestCase(UserType.Voter)]
        [TestCase(UserType.Admin)]
        public void When_adding_a_user(UserType type)
        {
            var user = new User { UserId = Guid.NewGuid(), Name = "Tester" };

            classUnderTest.AddUser(user, type);
            var result = classUnderTest.GetData();

            Assert.That(result.Users.Count, Is.EqualTo(1));
            Assert.That(result.Users[0].UserId, Is.EqualTo(user.UserId));
            Assert.That(result.Users[0].Name, Is.EqualTo(user.Name));
            Assert.That(result.Users[0].Type, Is.EqualTo(type));
        }

        [TestCase(ThumbVote.Up)]
        [TestCase(ThumbVote.Down)]
        [TestCase(ThumbVote.Sideways)]
        public void When_a_user_thumb_votes(ThumbVote vote)
        {
            var user = new User { UserId = Guid.NewGuid(), Name = "Tester" };
            classUnderTest.AddUser(user, UserType.Voter);

            classUnderTest.ThumbVote(user.UserId, vote);
            var result = classUnderTest.GetData();

            Assert.That(result.ThumbVotes[user.UserId], Is.EqualTo(vote));
        }

        [Test]
        public void When_an_unknown_user_thumb_votes()
        {
            classUnderTest.ThumbVote(Guid.NewGuid(), ThumbVote.Down);
            var result = classUnderTest.GetData();

            Assert.That(result.ThumbVotes.Count, Is.EqualTo(0));
        }

        [TestCase(UserType.Voter, VoteType.Thumbs)]
        [TestCase(UserType.Admin, VoteType.FistToFive)]
        public void When_switching_voting_type(UserType userType, VoteType expectedVoteType)
        {
            var user = new User { UserId = Guid.NewGuid(), Name = "Tester" };
            classUnderTest.AddUser(user, userType);

            classUnderTest.SetVotingType(user.UserId, VoteType.FistToFive);

            Assert.That(classUnderTest.GetData().VoteType, Is.EqualTo(expectedVoteType));
        }

        [TestCase(FistToFiveVote.Zero)]
        [TestCase(FistToFiveVote.One)]
        [TestCase(FistToFiveVote.Two)]
        [TestCase(FistToFiveVote.Three)]
        [TestCase(FistToFiveVote.Four)]
        [TestCase(FistToFiveVote.Five)]
        public void When_a_user_fist_to_five_votes(FistToFiveVote vote)
        {
            var user = new User { UserId = Guid.NewGuid(), Name = "Tester" };
            classUnderTest.AddUser(user, UserType.Voter);

            classUnderTest.FistToFiveVote(user.UserId, vote);
            var result = classUnderTest.GetData();

            Assert.That(result.FistToFiveVotes[user.UserId], Is.EqualTo(vote));
        }

        [Test]
        public void When_an_unknown_user_fist_to_five_votes()
        {
            classUnderTest.FistToFiveVote(Guid.NewGuid(), FistToFiveVote.Three);
            var result = classUnderTest.GetData();

            Assert.That(result.FistToFiveVotes.Count, Is.EqualTo(0));
        }

        [TestCase(UserType.Admin, "admin prompt", "admin prompt")]
        [TestCase(UserType.Voter, "voter prompt", "Do you concur?")]
        public void When_setting_a_prompt(UserType type, string newPrompt, string expectedPrompt)
        {
            var user = new User { UserId = Guid.NewGuid(), Name = "Tester" };
            classUnderTest.AddUser(user, type);

            classUnderTest.SetPrompt(user.UserId, newPrompt);
            var result = classUnderTest.GetData();

            Assert.That(result.Prompt, Is.EqualTo(expectedPrompt));
        }

        [Test]
        public void When_setting_a_prompt_the_old_votes_are_cleared()
        {
            var user = new User { UserId = Guid.NewGuid(), Name = "Tester" };
            classUnderTest.AddUser(user, UserType.Admin);
            classUnderTest.ThumbVote(user.UserId, ThumbVote.Sideways);
            classUnderTest.FistToFiveVote(user.UserId, FistToFiveVote.Three);
            Assert.That(classUnderTest.GetData().ThumbVotes.Count, Is.EqualTo(1));

            classUnderTest.SetPrompt(user.UserId, "New prompt");
            var result = classUnderTest.GetData();

            Assert.That(result.ThumbVotes.Count, Is.EqualTo(0));
            Assert.That(result.FistToFiveVotes.Count, Is.EqualTo(0));
        }

        [Test]
        public void When_setting_a_prompt_the_votes_are_no_longer_visible()
        {
            var user = new User { UserId = Guid.NewGuid(), Name = "Tester" };
            classUnderTest.AddUser(user, UserType.Admin);
            Assert.That(classUnderTest.GetData().VotesVisible, Is.False);

            classUnderTest.ThumbVote(user.UserId, ThumbVote.Sideways);
            Assert.That(classUnderTest.GetData().VotesVisible, Is.True);

            classUnderTest.SetPrompt(user.UserId, "New prompt");
            Assert.That(classUnderTest.GetData().VotesVisible, Is.False);
        }

        [Test]
        public void When_everyone_has_thumb_voted_the_results_are_made_visible()
        {
            var userA = new User { UserId = Guid.NewGuid(), Name = "A" };
            var userB = new User { UserId = Guid.NewGuid(), Name = "B" };
            classUnderTest.AddUser(userA, UserType.Voter);
            classUnderTest.AddUser(userB, UserType.Voter);

            classUnderTest.ThumbVote(userA.UserId, ThumbVote.Up);
            Assert.That(classUnderTest.GetData().VotesVisible, Is.False);

            classUnderTest.ThumbVote(userB.UserId, ThumbVote.Down);
            Assert.That(classUnderTest.GetData().VotesVisible, Is.True);
        }

        [Test]
        public void When_everyone_has_thumb_voted_but_it_was_a_different_voting_type()
        {
            var userA = new User { UserId = Guid.NewGuid(), Name = "A" };
            var userB = new User { UserId = Guid.NewGuid(), Name = "B" };
            classUnderTest.AddUser(userA, UserType.Admin);
            classUnderTest.AddUser(userB, UserType.Voter);
            classUnderTest.SetVotingType(userA.UserId, VoteType.FistToFive);

            classUnderTest.ThumbVote(userA.UserId, ThumbVote.Up);
            Assert.That(classUnderTest.GetData().VotesVisible, Is.False);

            classUnderTest.ThumbVote(userB.UserId, ThumbVote.Down);
            Assert.That(classUnderTest.GetData().VotesVisible, Is.False);
        }

        [Test]
        public void When_everyone_has_fist_to_five_voted_the_results_are_made_visible()
        {
            var userA = new User { UserId = Guid.NewGuid(), Name = "A" };
            var userB = new User { UserId = Guid.NewGuid(), Name = "B" };
            classUnderTest.AddUser(userA, UserType.Voter);
            classUnderTest.AddUser(userB, UserType.Admin);
            classUnderTest.SetVotingType(userB.UserId, VoteType.FistToFive);

            classUnderTest.FistToFiveVote(userA.UserId, FistToFiveVote.Two);
            Assert.That(classUnderTest.GetData().VotesVisible, Is.False);

            classUnderTest.FistToFiveVote(userB.UserId, FistToFiveVote.Four);
            Assert.That(classUnderTest.GetData().VotesVisible, Is.True);
        }

        [Test]
        public void When_everyone_has_fist_to_five_voted_but_it_was_a_different_voting_type()
        {
            var userA = new User { UserId = Guid.NewGuid(), Name = "A" };
            var userB = new User { UserId = Guid.NewGuid(), Name = "B" };
            classUnderTest.AddUser(userA, UserType.Admin);
            classUnderTest.AddUser(userB, UserType.Voter);
            classUnderTest.SetVotingType(userA.UserId, VoteType.Thumbs);

            classUnderTest.FistToFiveVote(userA.UserId, FistToFiveVote.Two);
            Assert.That(classUnderTest.GetData().VotesVisible, Is.False);

            classUnderTest.FistToFiveVote(userB.UserId, FistToFiveVote.Four);
            Assert.That(classUnderTest.GetData().VotesVisible, Is.False);
        }

        [TestCase(UserType.Admin, true)]
        [TestCase(UserType.Voter, false)]
        public void When_an_admin_makes_the_votes_visible(UserType type, bool expectedVisibility)
        {
            var userA = new User { UserId = Guid.NewGuid(), Name = "A" };
            var userB = new User { UserId = Guid.NewGuid(), Name = "B" };
            classUnderTest.AddUser(userA, UserType.Voter);
            classUnderTest.AddUser(userB, type);

            classUnderTest.ThumbVote(userA.UserId, ThumbVote.Up);
            Assert.That(classUnderTest.GetData().VotesVisible, Is.False);

            classUnderTest.MakeVotesVisible(userB.UserId);
            Assert.That(classUnderTest.GetData().VotesVisible, Is.EqualTo(expectedVisibility));
        }
    }
}
