using AuditLog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace AuditLog.SystemTests;

public class AuditLogWebApplicationFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("auditlog_test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    public string ConnectionString => _postgreSqlContainer.GetConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the default DbContext registration
            services.RemoveAll<DbContextOptions<AuditLogDbContext>>();
            services.RemoveAll<AuditLogDbContext>();

            // Register DbContext with testcontainer connection string
            services.AddDbContext<AuditLogDbContext>(options =>
                options.UseNpgsql(ConnectionString));
        });
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        await CreateDatabaseSchemaAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
    }

    private async Task CreateDatabaseSchemaAsync()
    {
        var options = new DbContextOptionsBuilder<AuditLogDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var context = new AuditLogDbContext(options);
        await context.Database.EnsureCreatedAsync();
    }
}