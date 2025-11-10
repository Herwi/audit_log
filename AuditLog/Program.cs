using AuditLog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AuditLogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RekrutacjaDb")));

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

var apiV1 = app.MapGroup("/api/v1");

apiV1.MapGet("/organizations/{organizationId}/user-activities", (string organizationId) => new List<UserActivity>())
    .WithName("GetUserActivities");

app.Run();

internal record UserActivity;

// Make the implicit Program class accessible to tests
public partial class Program
{
}