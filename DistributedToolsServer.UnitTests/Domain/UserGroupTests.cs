using System;
using System.Linq;
using DistributedToolsServer.Domain;
using NUnit.Framework;

namespace DistributedToolsServer.UnitTests.Domain
{
    public class UserGroupTests
    {
        private UserGroup classUnderTest;

        [SetUp]
        public void SetUp()
        {
            classUnderTest = new UserGroup();
        }

        [Test]
        public void When_there_are_no_users()
        {
            var result = classUnderTest.GetAllUsers();
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void When_adding_a_user()
        {
            var user = new User {UserId = Guid.NewGuid(), Name = "Mr. Test"};

            classUnderTest.AddUser(user, UserType.Admin);

            var result = classUnderTest.GetAllUsers();
            Assert.That(result.Single().Name, Is.EqualTo(user.Name));
            Assert.That(result.Single().UserId, Is.EqualTo(user.UserId));
            Assert.That(result.Single().Type, Is.EqualTo(UserType.Admin));
        }

        [Test]
        public void When_removing_a_user()
        {
            var userA = new User {UserId = Guid.NewGuid(), Name = "A"};
            var userB = new User {UserId = Guid.NewGuid(), Name = "B"};
            var userC = new User {UserId = Guid.NewGuid(), Name = "C"};

            classUnderTest.AddUser(userA, UserType.Voter);
            classUnderTest.AddUser(userB, UserType.Voter);
            classUnderTest.AddUser(userC, UserType.Voter);

            classUnderTest.RemoveUser(userB.UserId);

            var result = classUnderTest.GetAllUsers();
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Select(x => x.Name).OrderBy(x => x), Is.EqualTo(new [] { "A", "C" }));
        }
    }
}
