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
            var name = "Mr. Test";
            
            var userId = classUnderTest.AddUser(name, UserType.Admin);
            
            var result = classUnderTest.GetAllUsers();
            Assert.That(result.Single().Name, Is.EqualTo(name));
            Assert.That(result.Single().UserId, Is.EqualTo(userId));
            Assert.That(result.Single().Type, Is.EqualTo(UserType.Admin));
            Assert.That(userId, Is.Not.EqualTo(Guid.Empty));
        }
        
        [Test]
        public void When_removing_a_user()
        {
            classUnderTest.AddUser("A", UserType.Voter);
            var userId = classUnderTest.AddUser("B", UserType.Voter);
            classUnderTest.AddUser("C", UserType.Voter);

            classUnderTest.RemoveUser(userId);
            
            var result = classUnderTest.GetAllUsers();
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Select(x => x.Name).OrderBy(x => x), Is.EqualTo(new [] { "A", "C" }));
        }
    }
}