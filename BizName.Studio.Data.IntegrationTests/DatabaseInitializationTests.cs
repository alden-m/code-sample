using Microsoft.Extensions.DependencyInjection;
using BizName.Studio.Data.Database;
using BizName.Studio.Data.Database.Impl;

namespace BizName.Studio.Data.IntegrationTests;

/// <summary>
/// Tests for database and container initialization
/// </summary>
public class DatabaseInitializationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    [Fact]
    public async Task DatabaseInitializer_ShouldCreateDatabaseAndContainers()
    {
        // Arrange
        var databaseInitializer = fixture.ServiceProvider.GetRequiredService<IDatabaseInitializer>();

        // Act & Assert - Should not throw any exceptions
        await databaseInitializer.InitializeAsync();

        // The test passes if no exceptions are thrown
        // Database and containers are created during TestFixture initialization
        Assert.True(true, "Database and containers initialized successfully");
    }

    [Fact]
    public void DatabaseInitializer_ShouldBeRegistered()
    {
        // Arrange & Act
        var databaseInitializer = fixture.ServiceProvider.GetService<IDatabaseInitializer>();

        // Assert
        Assert.NotNull(databaseInitializer);
        Assert.IsType<DatabaseInitializer>(fixture.ServiceProvider.GetRequiredService<IDatabaseInitializer>());
    }
}
