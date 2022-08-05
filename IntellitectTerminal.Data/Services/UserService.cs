using IntelliTect.Coalesce.Models;
using IntellitectTerminal.Data;
using IntellitectTerminal.Data.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace IntellitectTerminal.Data.Services;

public class UserService : IUserService
{
    private AppDbContext Db { get; set; }

    public UserService(AppDbContext db)
    {
        this.Db = db;
    }
    public User InitializeFileSystem(Guid? userId)
    {
        User? user = Db.Users.Find(userId);
        if (user != null)
        {
            if (user.FileSystem is null)
            {
                user.FileSystem = JsonConvert.SerializeObject(CreateDefaultFileSystem(), new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            }
            return user;
        }
        else
        {
            return CreateAndSaveNewUser();
        }

    }
    public User CreateAndSaveNewUser()
    {
        User user = new() { UserId = Guid.NewGuid(), FileSystem = JsonConvert.SerializeObject(CreateDefaultFileSystem(), new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        })};
        Db.Users.Add(user);
        Db.SaveChanges();
        return user;
    }

    public static TreeNode<string> CreateDefaultFileSystem()
    {
        TreeNode<string> newFileSystem = new("/");
        newFileSystem.AddChild("home").AddChild("localuser").AddChild("readme.txt", true);
        return newFileSystem;
    }

}