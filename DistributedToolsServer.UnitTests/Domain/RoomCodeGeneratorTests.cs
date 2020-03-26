using System.Text.RegularExpressions;
using DistributedToolsServer.Domain;
using NUnit.Framework;

namespace DistributedToolsServer.UnitTests.Domain
{
    public class RoomCodeGeneratorTests
    {
        private RoomCodeGenerator classUnderTest;
        
        [SetUp]
        public void SetUp()
        {
            classUnderTest = new RoomCodeGenerator();
        }
        
        [Test]
        public void When_generating_a_code()
        {
            var code = classUnderTest.Generate();
            Assert.That(code.Length, Is.EqualTo(4));
            var pattern = new Regex("[2-9ABCFGHJKPRSTWXYZ]{4}");
            Assert.That(pattern.IsMatch(code), Is.True);
        }
        
        [Test]
        public void When_generating_a_code_it_is_different_from_the_last_code()
        {
            var code1 = classUnderTest.Generate();
            var code2 = classUnderTest.Generate();
            Assert.That(code1, Is.Not.EqualTo(code2));
        }
    }
}