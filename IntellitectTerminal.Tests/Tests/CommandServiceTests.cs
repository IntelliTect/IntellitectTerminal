using IntellitectTerminal.Data.Models;
using IntellitectTerminal.Data.Services;
using IntellitectTerminal.Tests.Helpers;

namespace IntellitectTerminal.Tests
{
    public class CommandServiceTests : UnitTestsBase
    {
        private ICommandService UnderTest { get; set; }
        private IUserService userService { get; set; }

        public CommandServiceTests()
        {
            UnderTest = Mocker.CreateInstance<CommandService>();
            userService = Mocker.CreateInstance<UserService>();
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

        [Fact]
        public void Submit_samplescript_SubmitSuccessfully()
        {
            Challenge challenge = TestData.AddNewPythonChallenge(1);
            User user = userService.InitializeFileSystem(null);
            UnderTest.Request(user.UserId);
            
            var fileName = @"../../../Samples/sample_script.py";
            using FileStream fs = File.OpenRead(fileName);
            IntelliTect.Coalesce.Models.File file = new(fs);
            UnderTest.SubmitFile(file, user.UserId);
            Submission submission = Db.Submissions.Where(x => x.User == user && x.Challenge == challenge).First();
            Assert.NotNull(submission.Content);
            Assert.Equal($"{file.Name}_{user.UserId}", submission.Content);
        }

        [Fact]
        public void Verify_samplescript_ReturnTrue()
        {
            Challenge challenge = TestData.AddNewPythonChallenge(1);
            User user = userService.InitializeFileSystem(null);
            UnderTest.Request(user.UserId);

            var fileName = @"../../../Samples/sample_script.py";
            using FileStream fs = File.OpenRead(fileName);
            IntelliTect.Coalesce.Models.File file = new(fs);
            UnderTest.SubmitFile(file, user.UserId);
            Assert.True(UnderTest.Verify(user.UserId));
        }

        [Fact]
        public void Submit_text_SubmitSuccessfully()
        {
            Challenge challenge = TestData.AddNewPythonChallenge(1);
            User user = userService.InitializeFileSystem(null);
            UnderTest.Request(user.UserId);

            var fileName = @"../../../Samples/sample_script.py";
            using FileStream fs = File.OpenRead(fileName);
            IntelliTect.Coalesce.Models.File file = new(fs);
            UnderTest.SubmitFile(file, user.UserId);
            Submission submission = Db.Submissions.Where(x => x.User == user && x.Challenge == challenge).First();
            Assert.NotNull(submission.Content);
            Assert.Equal($"{file.Name}_{user.UserId}", submission.Content);
        }

        [Fact]
        public void Submit_FullWorkflow_SubmitSuccessfully()
        {
            var fileName = @"../../../Samples/sample_script.py";
            using FileStream fs = File.OpenRead(fileName);
            IntelliTect.Coalesce.Models.File file = new(fs);
            _ =TestData.AddNewChallenge(1);
            Challenge LevelTwo = TestData.AddNewPythonChallenge(2);
            Challenge LevelThree = TestData.AddNewPythonChallenge(3);
            User user = userService.InitializeFileSystem(null);
            UnderTest.Request(user.UserId);
            Assert.Equal(1, Db.Submissions.Count());
            UnderTest.SubmitUserInput("Correct", user.UserId);
            Assert.Equal(1, Db.Submissions.Count());
            UnderTest.Verify(user.UserId);
            Assert.Equal("Correct", Db.Submissions.First().Content);
            Assert.Equal(true, Db.Submissions.First().IsCorrect);
            UnderTest.Request(user.UserId);
            Assert.Equal(2, Db.Submissions.Count());
            UnderTest.SubmitFile(file, user.UserId);
            Assert.Equal(2, Db.Submissions.Count());
            UnderTest.Verify(user.UserId);
            Assert.Equal(true, Db.Submissions.First().IsCorrect);
            Submission submission = Db.Submissions.Where(x => x.User == user && x.Challenge == LevelTwo).First();
            Assert.Equal($"{file.Name}_{user.UserId}", submission.Content);
            UnderTest.Request(user.UserId);
            Assert.Equal(3, Db.Submissions.Count());
            file.Name += "1";
            UnderTest.SubmitFile(file, user.UserId);
            Assert.Equal(3, Db.Submissions.Count());
            Assert.Equal(true, Db.Submissions.First().IsCorrect);
            submission = Db.Submissions.Where(x => x.User == user && x.Challenge == LevelThree).First();
            Assert.Equal($"{file.Name}_{user.UserId}", submission.Content);
        }
    }
}