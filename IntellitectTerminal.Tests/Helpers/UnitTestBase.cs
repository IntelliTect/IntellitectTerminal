using Moq.AutoMock;
using IntellitectTerminal.Data;
using IntellitectTerminal.Data.Models;
using System;
using System.Security.Claims;

namespace IntellitectTerminal.Tests.Helpers;

public abstract class UnitTestsBase : IDisposable
{
    protected AppDbContext Db => DbFixture.Db;
    protected TestDataUtils TestData { get; }

    private SqliteDbFixture DbFixture { get; }
    protected AutoMocker Mocker { get; } = new AutoMocker();

    protected UnitTestsBase()
    {
        DbFixture = new SqliteDbFixture();
        TestData = new TestDataUtils(Db);
        Mocker.Use(Db);
    }

    public void Dispose()
    {
        DbFixture.Dispose();
        GC.SuppressFinalize(this);
    }
}
