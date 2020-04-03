using System;
using DistributedToolsServer.Domain;
using NUnit.Framework;

namespace DistributedToolsServer.UnitTests.Domain
{
    public class DelegationSessionTests
    {
        private DecisionDelegationSession classUnderTest;

        [SetUp]
        public void SetUp()
        {
            classUnderTest = new DecisionDelegationSession(
                new UserGroup(),
                new DecisionDelegationItemGroup());
        }

        [Test]
        public void When_getting_data_for_a_new_session()
        {
            var result = classUnderTest.GetData();

            Assert.That(result.CurrentItemId, Is.EqualTo(Guid.Empty));
            Assert.That(result.Users.Count, Is.EqualTo(0));
            Assert.That(result.Items.Count, Is.EqualTo(0));
        }

        [Test]
        public void When_registering_users()
        {
            classUnderTest.AddUser(new User { UserId = Guid.NewGuid(), Name ="A"}, UserType.Admin);
            classUnderTest.AddUser(new User { UserId = Guid.NewGuid(), Name ="B"}, UserType.Voter);

            var result = classUnderTest.GetData();
            Assert.That(result.Users.Count, Is.EqualTo(2));
        }

        [Test]
        public void When_adding_items()
        {
            classUnderTest.AddItem("Item 1");
            classUnderTest.AddItem("Item B");

            var result = classUnderTest.GetData();

            Assert.That(result.Items.Count, Is.EqualTo(2));
        }

        [Test]
        public void When_voting_for_items()
        {
            var itemId = classUnderTest.AddItem("Test item");
            var userA = new User {UserId = Guid.NewGuid(), Name = "A"};
            var userB = new User {UserId = Guid.NewGuid(), Name = "B"};
            classUnderTest.AddUser(userA, UserType.Voter);
            classUnderTest.AddUser(userB, UserType.Voter);

            classUnderTest.Vote(itemId, userA.UserId, 1);

            var result = classUnderTest.GetData();
            Assert.That(result.Items[0].Votes[userA.UserId], Is.EqualTo(1));
            Assert.That(result.Items[0].IsVisible, Is.False);
        }

        [Test]
        public void When_voting_for_items_it_becomes_visible_once_all_users_have_voted()
        {
            var itemId = classUnderTest.AddItem("Test item");
            var userA = new User {UserId = Guid.NewGuid(), Name = "A"};
            var userB = new User {UserId = Guid.NewGuid(), Name = "B"};
            classUnderTest.AddUser(userA, UserType.Voter);
            classUnderTest.AddUser(userB, UserType.Voter);

            classUnderTest.Vote(itemId, userA.UserId, 2);
            classUnderTest.Vote(itemId, userB.UserId, 3);

            var result = classUnderTest.GetData();
            Assert.That(result.Items[0].Votes[userA.UserId], Is.EqualTo(2));
            Assert.That(result.Items[0].Votes[userB.UserId], Is.EqualTo(3));
            Assert.That(result.Items[0].IsVisible, Is.True);
        }

        [TestCase(UserType.Admin, true)]
        [TestCase(UserType.Voter, false)]
        public void When_making_an_item_visible_only_some_types_can_change_it(UserType type, bool expectedVisibility)
        {
            var itemId = classUnderTest.AddItem("Test item");
            var userA = new User {UserId = Guid.NewGuid(), Name = "A"};
            var userB = new User {UserId = Guid.NewGuid(), Name = "B"};
            classUnderTest.AddUser(userA, UserType.Voter);
            classUnderTest.AddUser(userB, type);

            classUnderTest.MakeVisible(itemId, userB.UserId);

            var result = classUnderTest.GetData();
            Assert.That(result.Items[0].IsVisible, Is.EqualTo(expectedVisibility));
        }

        [TestCase(UserType.Admin, true)]
        [TestCase(UserType.Voter, false)]
        public void When_selecting_current_item_only_an_admin_can_set_it(UserType type, bool expectedChange)
        {
            var itemId = classUnderTest.AddItem("Test item");
            var user = new User {UserId = Guid.NewGuid(), Name = "Test user"};
            classUnderTest.AddUser(user, type);

            classUnderTest.SetCurrentItem(itemId, user.UserId);

            var result = classUnderTest.GetData();
            Assert.That(result.CurrentItemId, Is.EqualTo(expectedChange ? itemId : Guid.Empty));
        }
    }
}
