using IntelliTect.Coalesce.Models;
using IntellitectTerminal.Data;
using IntellitectTerminal.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace IntellitectTerminal.Web;

public class CommandService : ICommandService
{
    private AppDbContext Db { get; set; }

    public CommandService(AppDbContext db)
    {
        this.Db = db;
    }

    public Challenge RequestCommand(Guid? userId)
    {
        // Check if userId is a Guid
        if (userId is null)
        {
            throw new InvalidOperationException($"userId {userId} is not a valid Guid");
        }
        User foundUser = Db.Users.Where(x => x.UserId == userId).FirstOrDefault() ?? CreateAndSaveNewUser();
        int highestCompletedLevel = Db.Submissions.Where(x => x.User == foundUser && x.IsCorrect == true)
            .Select(x => x.Challenge.Level).ToList().DefaultIfEmpty(0).Max();
        highestCompletedLevel++;
        return Db.Challenges.Where(x => x.Level == highestCompletedLevel).OrderBy(x=>EF.Functions.Random()).First();
    }

    private User CreateAndSaveNewUser()
    {
        User user = new User { UserId = Guid.NewGuid() };
        Db.Users.Add(user);
        Db.SaveChanges();
        return user;
    }
}
