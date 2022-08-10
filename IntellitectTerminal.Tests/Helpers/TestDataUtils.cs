using Hangfire;
using IntellitectTerminal.Data;
using IntellitectTerminal.Data.Models;
using IntellitectTerminal.Data.Services;
using Newtonsoft.Json;

namespace IntellitectTerminal.Tests.Helpers;

public class TestDataUtils
{
    public AppDbContext Db { get; }

    public TestDataUtils(AppDbContext db)
    {
        Db = db;
    }

    public User AddTwoDayOldUser()
    {
        User user = new()
        {
            UserId = Guid.NewGuid(),
            FileSystem = JsonConvert.SerializeObject(UserService.CreateDefaultFileSystem(), new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }),
            CreationTime = DateTime.Now.AddDays(-2)
        };
        Db.Users.Add(user);
        Db.SaveChanges();
        return user;
    }

    public User AddOneDayInFutureUser()
    {
        User user = new()
        {
            UserId = Guid.NewGuid(),
            FileSystem = JsonConvert.SerializeObject(UserService.CreateDefaultFileSystem(), new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }),
            CreationTime = DateTime.Now.AddDays(+1)
        };
        Db.Users.Add(user);
        Db.SaveChanges();
        return user;
    }

    public List<Challenge> AddNewChallenges()
    {
        List<Challenge> challenges = new();
        for (int i = 1; i < 5; i++)
        {
            Challenge newChallenge = new() { Question = "Correct Question", Answer = "Correct", CompilationLanguage = Challenge.CompilationLanguages.None, Level = i };
            challenges.Add(newChallenge);
            newChallenge = new() { Question = "Incorrect Question", Answer = "Incorrect", CompilationLanguage = Challenge.CompilationLanguages.None, Level = i };
            challenges.Add(newChallenge);
            newChallenge = new() { Question = "Correct Question", Answer = "Correct", CompilationLanguage = Challenge.CompilationLanguages.Python, Level = i };
            challenges.Add(newChallenge);
            newChallenge = new() { Question = "Incorrect Question", Answer = "Incorrect", CompilationLanguage = Challenge.CompilationLanguages.Python, Level = i };
            challenges.Add(newChallenge);
        }
        Db.Challenges.AddRange(challenges);
        return challenges;
    }

    public Challenge AddNewChallenge(int level)
    {
        Challenge newChallenge = new() { Question = "Correct Question", Answer = "Correct", CompilationLanguage = Challenge.CompilationLanguages.None, Level = level };
        Db.Challenges.Add(newChallenge);
        return newChallenge;
    }

    public Challenge AddNewPythonChallenge(int level)
    {
        Challenge newChallenge = new() { Question = "Correct Question", Answer = "Sucess", CompilationLanguage = Challenge.CompilationLanguages.Python, Level = level };
        Db.Challenges.Add(newChallenge);
        return newChallenge;
    }

    public User AddUserWithOnlyGuid()
    {
        User user = new() { UserId = Guid.NewGuid() };
        Db.Users.Add(user);
        Db.SaveChanges();
        return user;
    }

    public User AddFullUser()
    {
        User user = new()
        {
            UserId = Guid.NewGuid(),
            FileSystem = JsonConvert.SerializeObject(UserService.CreateDefaultFileSystem(), new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }),
            CreationTime = DateTime.Now
        };
        Db.Users.Add(user);
        Db.SaveChanges();
        return user;
    }

    public Submission AddSubmission(User user, Challenge challenge, bool? isCorrect)
    {
        Submission newSubmission = new() { Challenge = challenge, IsCorrect = isCorrect, User = user, Content = string.Empty };
        Db.Submissions.Add(newSubmission);
        Db.SaveChanges();
        return newSubmission;
    }
}
