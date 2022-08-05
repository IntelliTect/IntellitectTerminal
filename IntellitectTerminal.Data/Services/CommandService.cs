using IntellitectTerminal.Data.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

    public TreeNode Request(Guid? userId)
    {
        User foundUser = Db.Users.Where(x => x.UserId == userId).FirstOrDefault() ?? userService.CreateAndSaveNewUser();
        int highestCompletedLevel = GetHighestCompletedChallengeLevel(foundUser);
        TreeNode foundUsersFileSystem = GetUsersSystem(foundUser);
        highestCompletedLevel++;
        Submission? currentChallenge = Db.Submissions.Where(x => x.User == foundUser && x.Challenge.Level == highestCompletedLevel).FirstOrDefault();
        if (currentChallenge != null)
        {
            return foundUsersFileSystem;
        }
        else
        {
            foundUsersFileSystem.GetChild("challenges");
            return foundUsersFileSystem;
        }
    }

    private int GetHighestCompletedChallengeLevel(User? user)
    {
        return Db.Submissions.Where(x => x.User == user && x.IsCorrect == true)
                    .Select(x => x.Challenge.Level).ToList().DefaultIfEmpty(0).Max();
    }

    public Challenge GetNewChallenge(User user)
    {
        int highestCompletedLevel = Db.Submissions.Where(x => x.User == user && x.IsCorrect == true)
            .Select(x => x.Challenge.Level).ToList().DefaultIfEmpty(0).Max();
        highestCompletedLevel++;
        return Db.Challenges.Where(x => x.Level == highestCompletedLevel).OrderBy(x => EF.Functions.Random()).FirstOrDefault() ?? throw new InvalidOperationException("Challenge to be returned cannot be found");

    }

    private static TreeNode GetUsersSystem(User foundUser)
    {
        return JsonConvert.DeserializeObject<TreeNode>(foundUser.FileSystem) ?? throw new InvalidOperationException("Deserializing User File System Failed");
    }
}
