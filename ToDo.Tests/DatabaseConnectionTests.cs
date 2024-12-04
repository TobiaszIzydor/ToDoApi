using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System;
using System.Threading.Tasks;
using ToDo.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;

//Before running this test run docker compose from source project
public class DatabaseConnectionTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _databaseFixture;

    public DatabaseConnectionTests(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
    }

    [Fact]
    public async Task Database_Connection_Is_Successful()
    {
        // Arrange
        var options = _databaseFixture.DbContextOptions; //Get DbContext Options from fixture with connection string to database from docker

        // Act
        await using var context = new ToDoDbContext(options); //Creates a new ToDoDbContext instance with the specified options.
        var canConnect = await context.Database.CanConnectAsync(); //Checks if the database connection can be established asynchronously.

        // Assert
        Assert.True(canConnect);
    }
}

public class DatabaseFixture : IDisposable
{
    public DbContextOptions<ToDoDbContext> DbContextOptions { get; }

    public DatabaseFixture()
    {
        //Configure the connection string to point to Docker PostgreSQL container
        var connectionString = "Host=localhost;Port=5432;Database=todo;Username=postgres;Password=postgres;";

        DbContextOptions = new DbContextOptionsBuilder<ToDoDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        WaitForDatabase();
    }

    private void WaitForDatabase()
    {
        var retries = 10;
        var delay = TimeSpan.FromSeconds(5);
        for (var i = 0; i < retries; i++)
        {
            try
            {
                using (var context = new ToDoDbContext(DbContextOptions))
                {
                    context.Database.OpenConnection();
                    context.Database.CloseConnection();
                }
                break;  // Connection successful, exit loop
            }
            catch (Exception)
            {
                if (i == retries - 1) throw;
                Thread.Sleep(delay);
            }
        }
    }

    public void Dispose()
    {
    }
}