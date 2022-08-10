using Hangfire;
using IntellitectTerminal.Data.Models;
using IntellitectTerminal.Data.Services;
using IntellitectTerminal.Tests.Helpers;
using IntellitectTerminal.Web.Hangfire;
using Moq;
using Newtonsoft.Json;

namespace IntellitectTerminal.Tests
{
    public class RecurringJobServiceTests : UnitTestsBase
    {
        private RecurringJobService UnderTest { get; set; }

        public RecurringJobServiceTests()
        {
            UnderTest = Mocker.CreateInstance<RecurringJobService>();
        }

        [Fact]
        public async void RemoveExpiredUsers_AddTwoExpiredUsersAndTwoNotExpired_OnlyTwoNotExpiredRemain()
        {
            User NonexistentUser1 = TestData.AddTwoDayOldUser();
            User NonexistentUser2 = TestData.AddTwoDayOldUser();
            User user1= TestData.AddFullUser();
            User user2 = TestData.AddFullUser();
            User user3 = TestData.AddOneDayInFutureUser();
            await UnderTest.RemoveExpiredUsers();

            Assert.Equal(3, Db.Users.Count());
            Assert.Equal(user1, Db.Users.Where(x => x == user1).First());
            Assert.Equal(user2, Db.Users.Where(x => x == user2).First());
            Assert.Equal(user3, Db.Users.Where(x => x == user3).First());
            Assert.Empty(Db.Users.Where(x => x == NonexistentUser1));
            Assert.Empty(Db.Users.Where(x => x == NonexistentUser2));
        }
    }
}