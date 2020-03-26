using System;
using DistributedToolsServer.Domain;
using NUnit.Framework;

namespace DistributedToolsServer.UnitTests.Domain
{
    public class ItemRepositoryTests
    {
        private ItemRepository classUnderTest;
        
        [SetUp]
        public void SetUp()
        {
            classUnderTest = new ItemRepository();
        }

        [Test]
        public void When_there_are_no_items()
        {
            var result = classUnderTest.GetAllItems();

            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void When_adding_an_item()
        {
            var description = "This new item";

            var itemId = classUnderTest.AddItem(description);

            var items = classUnderTest.GetAllItems();
            Assert.That(items.Count, Is.EqualTo(1));
            Assert.That(items[0].ItemId, Is.EqualTo(itemId));
            Assert.That(items[0].Description, Is.EqualTo(description));
            Assert.That(items[0].Votes.Count, Is.EqualTo(0));
            Assert.That(items[0].IsVisible, Is.False);
        }
        
        [Test]
        public void When_removing_an_item()
        {
            var itemId = classUnderTest.AddItem("This item");

            classUnderTest.Remove(itemId);

            var items = classUnderTest.GetAllItems();
            Assert.That(items.Count, Is.EqualTo(0));
        }

        [Test]
        public void When_registering_a_user_vote()
        {
            var itemId = classUnderTest.AddItem("Item for voting");
            var userId = Guid.NewGuid();

            var result = classUnderTest.RegisterVote(itemId, userId, 3);

            Assert.That(result, Is.EqualTo(1));
            var items = classUnderTest.GetAllItems();
            Assert.That(items[0].Votes[userId], Is.EqualTo(3));
        }

        [Test]
        public void When_registering_multiple_votes()
        {
            var itemId = classUnderTest.AddItem("Item for voting");
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var userId3 = Guid.NewGuid();

            classUnderTest.RegisterVote(itemId, userId1, 4);
            classUnderTest.RegisterVote(itemId, userId2, 5);
            var result = classUnderTest.RegisterVote(itemId, userId3, 4);
            
            Assert.That(result, Is.EqualTo(3));
            var items = classUnderTest.GetAllItems();
            Assert.That(items[0].Votes[userId1], Is.EqualTo(4));
            Assert.That(items[0].Votes[userId2], Is.EqualTo(5));
            Assert.That(items[0].Votes[userId3], Is.EqualTo(4));
        }
        
        [Test]
        public void When_updating_a_user_vote()
        {
            var itemId = classUnderTest.AddItem("Item for voting");
            var userId = Guid.NewGuid();

            classUnderTest.RegisterVote(itemId, userId, 3);
            var result = classUnderTest.RegisterVote(itemId, userId, 2);

            Assert.That(result, Is.EqualTo(1));
            var items = classUnderTest.GetAllItems();
            Assert.That(items[0].Votes[userId], Is.EqualTo(2));
        }

        [Test]
        public void When_registering_a_vote_for_an_unknown_item()
        {
            var result = classUnderTest.RegisterVote(Guid.NewGuid(), Guid.NewGuid(), 7);
            
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void When_updating_visibility_for_an_item()
        {
            var itemId = classUnderTest.AddItem("Some item");

            var result = classUnderTest.SetVisibility(itemId, true);
            
            Assert.That(result, Is.True);
            
            result = classUnderTest.SetVisibility(itemId, false);
            
            Assert.That(result, Is.False);
        }

        [Test]
        public void When_updating_visibility_for_an_unknown_item()
        {
            var result = classUnderTest.SetVisibility(Guid.NewGuid(), true);
            
            Assert.That(result, Is.False);
        }
    }
}