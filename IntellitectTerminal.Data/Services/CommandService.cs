using IntellitectTerminal.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace IntellitectTerminal.Data.Services;

public class CommandService : ICommandService
{
    private AppDbContext Db { get; set; }
    private UserService userService { get; set; }

    public CommandService(AppDbContext db)
    {
        this.Db = db;
        this.userService = new UserService(db);
    }

    public Challenge Request(Guid? userId)
    {
        Db.Challenges.Add(new Challenge { Level = 1, CompilationLanguage = Challenge.CompilationLanguages.None, Answer = "InCorrect", Question = "Incorrect Question" });
        User foundUser = Db.Users.Where(x => x.UserId == userId).FirstOrDefault() ?? userService.CreateAndSaveNewUser();
        int highestCompletedLevel = Db.Submissions.Where(x => x.User == foundUser && x.IsCorrect == true)
            .Select(x => x.Challenge.Level).ToList().DefaultIfEmpty(0).Max();
        highestCompletedLevel++;
        return Db.Challenges.Where(x => x.Level == highestCompletedLevel).OrderBy(x => EF.Functions.Random()).First();
    }
}
