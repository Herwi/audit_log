using AuditLog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace AuditLog.SystemTests;

/// <summary>
/// Collection definition for database tests that must run sequentially.
/// xUnit runs tests in different collections in parallel, but tests within
/// the same collection run sequentially.
/// </summary>
[CollectionDefinition(AuditLogWebApplicationFixture.CollectionName, DisableParallelization = true)]
public class AuditLogWebApplicationFixtureCollection : ICollectionFixture<AuditLogWebApplicationFixture>
{
}

public class AuditLogWebApplicationFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string CollectionName = nameof(AuditLogWebApplicationFixture);

    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("auditlog_test")
        .WithUsername("test")
        .WithPassword("test")
        .WithReuse(true)
        .WithName("auditlog_test_container")
        .Build();

    public string ConnectionString => _postgreSqlContainer.GetConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the default DbContext registration
            services.RemoveAll<DbContextOptions<RekrutacjaDbContext>>();
            services.RemoveAll<RekrutacjaDbContext>();

            // Register DbContext with testcontainer connection string
            services.AddDbContext<RekrutacjaDbContext>(options =>
                options.UseNpgsql(ConnectionString));
        });
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        await RecreateDatabaseAsync();
        await SeedDatabaseAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
    }

    private async Task RecreateDatabaseAsync()
    {
        var options = new DbContextOptionsBuilder<RekrutacjaDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var context = new RekrutacjaDbContext(options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    private async Task SeedDatabaseAsync()
    {
        var options = new DbContextOptionsBuilder<RekrutacjaDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var context = new RekrutacjaDbContext(options);
        await DatabaseSeeder.SeedAsync(context);
    }
}