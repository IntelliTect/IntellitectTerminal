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
        public void Request_InvalidGuid_ReturnsFileSystemWithLevelOneQuestion()
        {
            TestData.AddNewChallenges();
            string fileSystem = UnderTest.Request(null);
            TreeNode<string> deserializedFileSystem = TreeNode<string>.DeserializeFileSystem(fileSystem);
            Assert.NotNull(TreeNode<string>.GetChild(deserializedFileSystem, "challenges", false));
            Assert.NotNull(TreeNode<string>.GetChild(deserializedFileSystem, "challenge_1.txt", true));
        }

        [Fact]
        public void Request_ExistingUser_ReturnsLevelOneQuestion()
        {
            TestData.AddNewChallenges();
            User user = TestData.AddFullUser();
            string fileSystem = UnderTest.Request(user.UserId);
            TreeNode<string> deserializedFileSystem = TreeNode<string>.DeserializeFileSystem(fileSystem);
            Assert.NotNull(TreeNode<string>.GetChild(deserializedFileSystem, "challenges", false));
            Assert.NotNull(TreeNode<string>.GetChild(deserializedFileSystem, "challenge_1.txt", true));
        }

        [Fact]
        public void Request_ExistingUserWithLevelOneSubmission_ReturnsFileSystemWithLevelTwoQuestion()
        {
            Challenge challenge = TestData.AddNewChallenges().Where(x => x.Level == 1).First();
            User user = TestData.AddFullUser();
            TestData.AddSubmission(user, challenge, true);
            string fileSystem = UnderTest.Request(user.UserId);
            TreeNode<string> deserializedFileSystem = TreeNode<string>.DeserializeFileSystem(fileSystem);
            Assert.NotNull(TreeNode<string>.GetChild(deserializedFileSystem, "challenges", false));
            Assert.NotNull(TreeNode<string>.GetChild(deserializedFileSystem, "challenge_2.txt", true));
        }

        [Fact]
        public void Request_ExistingUserWithAllThreeLevels_ReturnsNull()
        {
            User user = TestData.AddFullUser();
            Challenge challenge = TestData.AddNewChallenges().Where(x => x.Level == 3).First();
            TestData.AddSubmission(user, challenge, true);
            string fileSystem = UnderTest.Request(user.UserId);
            TreeNode<string> deserializedFileSystem = TreeNode<string>.DeserializeFileSystem(fileSystem);
            Assert.NotNull(TreeNode<string>.GetChild(deserializedFileSystem, "challenges", false));
            Assert.Throws<InvalidOperationException>(() => TreeNode<string>.GetChild(deserializedFileSystem, "challenge_4.txt", true));
        }

        [Fact]
        public void Cat_ExistingUserWithSecondChallenge_ReturnsChallengeAnswer()
        {
            Challenge challenge = TestData.AddNewChallenges().Where(x => x.Level == 2).First();
            User user = TestData.AddFullUser();
            TestData.AddSubmission(user, challenge, null);
            Assert.Equal(challenge.Question, UnderTest.Cat(user.UserId, "challenge_2.txt"));
        }

        [Fact]
        public void Cat_ExistingUserWithFirstChallengeAfterRequest_ReturnsChallengeAnswer()
        {
            Challenge challenge = TestData.AddNewChallenge(1);
            User user = TestData.AddFullUser();
            UnderTest.Request(user.UserId);
            Assert.Equal(challenge.Question, UnderTest.Cat(user.UserId, "challenge_1.txt"));
        }

        [Fact]
        public void Cat_Readme_ReturnsReadmeContents()
        {
            User user = TestData.AddFullUser();
            Assert.StartsWith("Welcome to the Intellitect Terminal!", UnderTest.Cat(user.UserId, "readme.txt"));
        }
    }
}