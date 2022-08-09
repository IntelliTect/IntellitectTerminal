using IntelliTect.Coalesce;
using IntelliTect.Coalesce.Models;
using IntellitectTerminal.Data.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

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
            // If it can't find an existing file which should be the case, then add one.
            catch (InvalidOperationException)
            {
                challengesFolder.AddChild($"challenge_{highestCompletedLevel}.txt", true);
                Db.Submissions.Add(new() { User = foundUser, Challenge = GetNewChallenge(foundUser), Content = null, IsCorrect = null });
                foundUser.FileSystem = TreeNode<string>.SerializeFileSystem(foundUsersFileSystem);
                Db.SaveChanges();
                return foundUser.FileSystem;
            }
            throw new InvalidOperationException($"challenge_{highestCompletedLevel}.txt unexpectedly exists");
        }
    }

    public string Cat(Guid userId, string fileName)
    {
        User foundUser = Db.Users.Where(x => x.UserId == userId).FirstOrDefault() ?? throw new InvalidOperationException($"User:{userId} not found");
        switch (fileName)
        {
            case string x when x.StartsWith("challenge_"):
                if (!int.TryParse(fileName[10].ToString(), out int challengeNumber))
                {
                    throw new InvalidOperationException($"challenge number on file is unexpectedly not an integer. Value is {fileName[10]}");
                }
                IQueryable<Submission>? usersSubmission = Db.Submissions.Include(x=>x.Challenge).Where(x => x.User == foundUser && x.Challenge.Level == challengeNumber);
                return usersSubmission.First().Challenge.Question;

            case string x when x.StartsWith("readme.txt"):
                return "Welcome to the Intellitect Terminal!\nIf it is your first time, run help to learn all of the availabe commands";
            default:
                return "Error: File contents cannot be displayed with cat";
        }
    }

    public string Progress(Guid userId)
    {
        User foundUser = Db.Users.Where(x => x.UserId == userId).FirstOrDefault() ?? throw new InvalidOperationException($"User:{userId} not found");
        int highestLevel = GetHighestCompletedChallengeLevel(foundUser);
        return $"You have completed {highestLevel} out of 3 challenge levels!";
    }

    public bool Verify(Guid userId)
    {
        int highestCompletedLevel = GetHighestCompletedChallengeLevel(Db.Users.Where(x => x.UserId == userId).First());
        highestCompletedLevel++;
        Submission submission = Db.Submissions.Include(x=>x.Challenge).Where(x => x.User.UserId == userId && x.Challenge.Level == highestCompletedLevel).First();
        switch (submission.Challenge.CompilationLanguage)
        {
            case Challenge.CompilationLanguages.None:
                submission.IsCorrect = true;
                Db.SaveChanges();
                break;

            case Challenge.CompilationLanguages.Python:
                string fileName = submission.Content ?? throw new InvalidOperationException("No submission content found");

                Process? process = Process.Start(new ProcessStartInfo(@"python", fileName)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                submission.IsCorrect = output.ToLower().Trim().Equals(submission.Challenge.Answer?.ToLower().Trim());
                Db.SaveChanges();
                break;
            default:
                break;
        }
        return submission.IsCorrect ?? false;
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

    [Coalesce]
    public async Task SubmitFile(IFile file, Guid userId)
    {
        if (file.Content == null) return;
        MemoryStream ms = new MemoryStream();
        file.Content.CopyTo(ms);
        var bytearray = ms.ToArray();
        string filename = $"{file.Name}_{userId}";
        System.IO.File.WriteAllBytes(filename, bytearray);
        int highestCompletedLevel = GetHighestCompletedChallengeLevel(Db.Users.Where(x => x.UserId == userId).First());
        highestCompletedLevel++;
        Submission submission = Db.Submissions.Where(x => x.User.UserId == userId && x.Challenge.Level == highestCompletedLevel).First();
        submission.Content = filename;
        await Db.SaveChangesAsync();
    }
    [Coalesce]
    public async Task SubmitUserInput(string input, Guid userId)
    {
        int highestCompletedLevel = GetHighestCompletedChallengeLevel(Db.Users.Where(x => x.UserId == userId).First());
        highestCompletedLevel++;
        Submission submission = Db.Submissions.Where(x => x.User.UserId == userId && x.Challenge.Level == highestCompletedLevel).First();
        submission.Content = input;
        await Db.SaveChangesAsync();
    }
}
