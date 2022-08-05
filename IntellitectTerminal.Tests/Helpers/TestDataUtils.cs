using IntellitectTerminal.Data;
using IntellitectTerminal.Data.Models;
using System;

namespace IntellitectTerminal.Tests.Helpers;

public class TestDataUtils
{
    private AppDbContext Db { get; }

    public TestDataUtils(AppDbContext db)
    {
        Db = db;
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
        }
        Db.Challenges.AddRange(challenges);
        return challenges;
    }
    public User AddUser()
    {
        User user = new() { UserId = Guid.NewGuid() };
        Db.Users.Add(user);
        Db.SaveChanges();
        return user;
    }

    public Submission AddSubmission(User user, Challenge challenge, bool isCorrect)
    {
        Submission newSubmission = new() { Challenge = challenge, IsCorrect = isCorrect, User = user, Content = string.Empty };
        Db.Submissions.Add(newSubmission);
        Db.SaveChanges();
        return newSubmission;
    }
}
