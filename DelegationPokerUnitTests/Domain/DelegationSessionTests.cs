using System;
using DelegationServer.Domain;
using NUnit.Framework;

namespace DelegationPokerUnitTests.Domain
{
    public class DelegationSessionTests
    {
        private DelegationSession classUnderTest;
        
        [SetUp]
        public void SetUp()
        {
            classUnderTest = new DelegationSession(
                new UserRepository(),
                new ItemRepository());
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
            classUnderTest.RegisterUser("A", UserType.Admin);
            classUnderTest.RegisterUser("B", UserType.Voter);
            
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
            var userA = classUnderTest.RegisterUser("A", UserType.Voter);
            var userB = classUnderTest.RegisterUser("B", UserType.Voter);

            classUnderTest.Vote(itemId, userA, 1);
            
            var result = classUnderTest.GetData();
            Assert.That(result.Items[0].Votes[userA], Is.EqualTo(1));
            Assert.That(result.Items[0].IsVisible, Is.False);
        }
        
        [Test]
        public void When_voting_for_items_it_becomes_visible_once_all_users_have_voted()
        {
            var itemId = classUnderTest.AddItem("Test item");
            var userA = classUnderTest.RegisterUser("A", UserType.Voter);
            var userB = classUnderTest.RegisterUser("B", UserType.Voter);

            classUnderTest.Vote(itemId, userA, 2);
            classUnderTest.Vote(itemId, userB, 3);
            
            var result = classUnderTest.GetData();
            Assert.That(result.Items[0].Votes[userA], Is.EqualTo(2));
            Assert.That(result.Items[0].Votes[userB], Is.EqualTo(3));
            Assert.That(result.Items[0].IsVisible, Is.True);
        }

        [TestCase(UserType.Admin, true)]
        [TestCase(UserType.Voter, false)]
        public void When_making_an_item_visible_only_some_types_can_change_it(UserType type, bool expectedVisibility)
        {
            var itemId = classUnderTest.AddItem("Test item");
            var userA = classUnderTest.RegisterUser("A", UserType.Voter);
            var userB = classUnderTest.RegisterUser("B", type);

            classUnderTest.MakeVisible(itemId, userB);

            var result = classUnderTest.GetData();
            Assert.That(result.Items[0].IsVisible, Is.EqualTo(expectedVisibility));
        }
        
        [TestCase(UserType.Admin, true)]
        [TestCase(UserType.Voter, false)]
        public void When_selecting_current_item_only_an_admin_can_set_it(UserType type, bool expectedChange)
        {
            var itemId = classUnderTest.AddItem("Test item");
            var user = classUnderTest.RegisterUser("Test user", type);

            classUnderTest.SetCurrentItem(itemId, user);

            var result = classUnderTest.GetData();
            Assert.That(result.CurrentItemId, Is.EqualTo(expectedChange ? itemId : Guid.Empty));
        }
    }
}