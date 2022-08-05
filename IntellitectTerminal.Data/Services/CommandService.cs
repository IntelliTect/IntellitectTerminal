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

    public string Request(Guid? userId)
    {
        User foundUser = Db.Users.Where(x => x.UserId == userId).FirstOrDefault() ?? userService.CreateAndSaveNewUser();
        int highestCompletedLevel = GetHighestCompletedChallengeLevel(foundUser);
        TreeNode<string> foundUsersFileSystem = TreeNode<string>.DeserializeFileSystem(foundUser);
        if (highestCompletedLevel >= 3)
        {
            return TreeNode<string>.SerializeFileSystem(foundUsersFileSystem);
        }
        highestCompletedLevel++;
        TreeNode<string> challengesFolder = TreeNode<string>.GetChild(foundUsersFileSystem, "challenges", false);
        Submission? currentChallenge = Db.Submissions.Where(x => x.User == foundUser && x.Challenge.Level == highestCompletedLevel).FirstOrDefault();
        if (currentChallenge != null)
        {
            TreeNode<string> challengeFile = TreeNode<string>.GetChild(foundUsersFileSystem, $"challenge_{highestCompletedLevel}.txt", true);
            return TreeNode<string>.SerializeFileSystem(foundUsersFileSystem);
        }
        else
        {
            try
            {
                TreeNode<string>.GetChild(foundUsersFileSystem, $"challenge_{highestCompletedLevel}.txt", true);
            }
            catch (InvalidOperationException)
            {
                challengesFolder.AddChild($"challenge_{highestCompletedLevel}.txt", true);
                foundUser.FileSystem = TreeNode<string>.SerializeFileSystem(foundUsersFileSystem);
                Db.SaveChanges();
                return foundUser.FileSystem;
            }
            throw new InvalidOperationException($"challenge_{highestCompletedLevel}.txt unexpectidly exists");
        }
    }

    private int GetHighestCompletedChallengeLevel(User? user)
    {
        return Db.Submissions.AsNoTracking().Where(x => x.User == user && x.IsCorrect == true)
                    .Select(x => x.Challenge.Level).ToList().DefaultIfEmpty(0).Max();
    }

    public Challenge GetNewChallenge(User user)
    {
        int highestCompletedLevel = Db.Submissions.AsNoTracking().Where(x => x.User == user && x.IsCorrect == true)
            .Select(x => x.Challenge.Level).ToList().DefaultIfEmpty(0).Max();
        highestCompletedLevel++;
        return Db.Challenges.Where(x => x.Level == highestCompletedLevel).OrderBy(x => EF.Functions.Random()).FirstOrDefault() ?? throw new InvalidOperationException("Challenge to be returned cannot be found");
    }
}
