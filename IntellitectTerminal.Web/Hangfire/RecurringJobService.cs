using Hangfire;
using IntellitectTerminal.Data;
using IntellitectTerminal.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace IntellitectTerminal.Web.Hangfire;

public class RecurringJobService
{
    private AppDbContext Db { get; }

    public RecurringJobService(
        AppDbContext db
    )
    {
        Db = db ?? throw new ArgumentNullException(nameof(db));
    }

    [AutomaticRetry(Attempts = 1)]
    public async Task RemoveExpiredUsers()
    {
        DateTime now = DateTime.Now;
        List<User> OldUsers = Db.Users.Where(x => x.CreationTime < now.AddHours(-24)).ToList();
        foreach (User user in OldUsers)
        {
            Db.Users.Remove(user);
        }

        await Db.SaveChangesAsync();
    }
}
