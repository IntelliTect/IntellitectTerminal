using IntellitectTerminal.Data.Models;
using IntellitectTerminal.Data.Services;
using IntellitectTerminal.Tests.Helpers;

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
        public void InitializeFileSystem_NullUser_ReturnNewUser()
        {
            TestData.AddNewChallenges();
            User newUser = UnderTest.InitializeFileSystem(null);
            Assert.NotNull(newUser);
            // You can't Assert.NotNull a value type (xUnit2002 https://xunit.net/xunit.analyzers/rules/xUnit2002)
            Assert.True(Guid.TryParse(newUser.UserId.ToString(), out _));
        }

        [Fact]
        public void InitializeFileSystem_ExistingUser_ReturnExistingUser()
        {
            User user = TestData.AddUser();
            User existingUser = UnderTest.InitializeFileSystem(user);
            Assert.Equal(user, existingUser);
        }
    }
}