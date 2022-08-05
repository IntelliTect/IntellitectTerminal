using IntelliTect.Coalesce.Models;
using IntellitectTerminal.Data;
using IntellitectTerminal.Data.Models;

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
        }

        Challenge challenge = new();

        User foundUser = Db.Users.Where(x => x.UserId == userId).FirstOrDefault() ?? CreateAndSaveNewUser();

        // Interpret file system to determine what challenge they are on
        challenge.Level = 1;
        challenge.Question = "This is the question";
        challenge.Answer = "You did it right!";
        challenge.CompilationLanguage = Challenge.CompilationLanguages.None;
        return challenge;
    }

    private User CreateAndSaveNewUser()
    {
        User user = new User { UserId = Guid.NewGuid() };
        Db.Users.Add(user);
        Db.SaveChanges();
        return user;
    }
}
