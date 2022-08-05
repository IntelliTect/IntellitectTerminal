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
        highestCompletedLevel++;
        TreeNode<string> challengesFolder = foundUsersFileSystem.Children.Where(x => x.Name == "challenges" && x.isFile is false).First();
        Submission? currentChallenge = Db.Submissions.Where(x => x.User == foundUser && x.Challenge.Level == highestCompletedLevel).FirstOrDefault();
        if (currentChallenge != null)
        {
            TreeNode<string> challengeFile = foundUsersFileSystem.Children.Where(x => x.Name == $"challenge_{highestCompletedLevel}.txt" && x.isFile is false).First();
            return TreeNode<string>.SerializeFileSystem(foundUsersFileSystem);
        }
        else
        {
            TreeNode<string> challengeFile = foundUsersFileSystem.Children.Where(x => x.Name == $"challenge_{highestCompletedLevel}.txt" && x.isFile is false).First();
            if (challengeFile != null)
            {
                throw new InvalidOperationException($"challenge_{highestCompletedLevel}.txt does not exist");
            }
            challengesFolder.AddChild($"challenge_{highestCompletedLevel}.txt", true);
            return TreeNode<string>.SerializeFileSystem(foundUsersFileSystem);
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
}
