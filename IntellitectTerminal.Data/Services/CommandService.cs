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
        Challenge challenge = new();
        challenge.ChallengeId = 1;
        challenge.Level = 1;
        challenge.Question = "This is the question";
        challenge.Answer = "You did it right!";
        challenge.CompilationLanguage = Challenge.CompilationLanguages.None;
        return challenge;
    }
}
