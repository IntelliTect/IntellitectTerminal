using IntelliTect.Coalesce.Models;
using IntellitectTerminal.Data;
using IntellitectTerminal.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace IntellitectTerminal.Data.Services;

public class UserService : IUserService
{
    private AppDbContext Db { get; set; }

    public UserService(AppDbContext db)
    {
        this.Db = db;
    }
    public User InitializeFileSystem(User? user)
    {
        if (user == null || Db.Users.Find(user.UserId) == null)
        {
            return CreateAndSaveNewUser();
        }
        else
        {
            return user;
        }

    }
    public User CreateAndSaveNewUser()
    {
        User user = new() { UserId = Guid.NewGuid(), FileSystem = CreateDefaultFileSystem()};
        Db.Users.Add(user);
        Db.SaveChanges();
        return user;
    }

    private string CreateDefaultFileSystem()
    {
        return "";
    }
}
