using IntellitectTerminal.Data.Models;
using IntellitectTerminal.Data.Services;
using IntellitectTerminal.Tests.Helpers;

namespace IntellitectTerminal.Tests
{
    public class CommandServiceTests : UnitTestsBase
    {
        private ICommandService UnderTest { get; set; }

        public CommandServiceTests()
        {
            UnderTest = Mocker.CreateInstance<CommandService>();
        }

        [Fact]
        public void Request_InvalidGuid_ReturnsLevelOneQuestion()
        {
            TestData.AddNewChallenges();
            Assert.Equal(1, UnderTest.Request(null).Level);
        }

        [Fact]
        public void Request_ExistingUser_ReturnsLevelOneQuestion()
        {
            TestData.AddNewChallenges();
            Assert.Equal(1, UnderTest.Request(TestData.AddUserWithOnlyGuid().UserId).Level);
        }

        [Fact]
        public void Request_ExistingUserWithLevelOneSubmission_ReturnsLevelTwoQuestion()
        {
            Challenge challenge = TestData.AddNewChallenges().Where(x => x.Level == 1).First();
            User user = TestData.AddUserWithOnlyGuid();
            TestData.AddSubmission(user, challenge, true);
            Assert.Equal(2, UnderTest.Request(user.UserId).Level);
        }

        [Fact]
        public void Request_EmptyDatabase_ReturnsException()
        {
            Assert.Throws<InvalidOperationException>(() => UnderTest.Request(Guid.NewGuid()));
        }
    }
}