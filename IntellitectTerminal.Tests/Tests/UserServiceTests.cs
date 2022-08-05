using IntellitectTerminal.Data.Models;
using IntellitectTerminal.Data.Services;
using IntellitectTerminal.Tests.Helpers;
using Newtonsoft.Json;

namespace IntellitectTerminal.Tests
{
    public class UserServiceTests : UnitTestsBase
    {
        private IUserService UnderTest { get; set; }

        public UserServiceTests()
        {
            UnderTest = Mocker.CreateInstance<UserService>();
        }

        [Fact]
        public void InitializeFileSystem_NullUser_ReturnFullNewUser()
        {
            TestData.AddNewChallenges();
            User newUser = UnderTest.InitializeFileSystem(null);
            Assert.NotNull(newUser);
            // You can't Assert.NotNull a value type (xUnit2002 https://xunit.net/xunit.analyzers/rules/xUnit2002)
            Assert.True(Guid.TryParse(newUser.UserId.ToString(), out _));
            Assert.Equal(JsonConvert.SerializeObject(UserService.CreateDefaultFileSystem(), new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }), newUser.FileSystem);
        }

        [Fact]
        public void InitializeFileSystem_ExistingUser_ReturnExistingUser()
        {
            User user = TestData.AddFullUser();
            User existingUser = UnderTest.InitializeFileSystem(user.UserId);
            Assert.Equal(user, existingUser);
        }

        [Fact]
        public void InitializeFileSystem_ExistingUserWithoutFileSystem_ReturnUserWithDefaultFileSystem()
        {
            User user = TestData.AddUserWithOnlyGuid();
            User existingUser = UnderTest.InitializeFileSystem(user.UserId);
            Assert.Equal(user.UserId, existingUser.UserId);
            Assert.Equal(JsonConvert.SerializeObject(UserService.CreateDefaultFileSystem(), new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }), existingUser.FileSystem);
        }
    }
}