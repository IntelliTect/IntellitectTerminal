using IntellitectTerminal.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace IntellitectTerminal.Tests.Helpers;

/// <summary>
/// Provides an in-memory SQLite database for <see cref="AppDbContext"/>.
/// </summary>
public class SqliteDbFixture : IDisposable
{
    public AppDbContext Db { get; private set; }
    private SqliteConnection Connection { get; }

    public SqliteDbFixture()
    {
        Connection = new SqliteConnection("DataSource=:memory:");
        Connection.Open();

        DbContextOptionsBuilder<AppDbContext> dbOptionBuilder = new();
        dbOptionBuilder.UseSqlite(Connection);

        Db = new AppDbContext(dbOptionBuilder.Options);
        Db.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Db.Dispose();
        Connection.Dispose();
        GC.SuppressFinalize(this);
    }
}
