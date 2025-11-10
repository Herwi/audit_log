using AuditLog.Application.Endpoints;
using AuditLog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<RekrutacjaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RekrutacjaDb")));

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

// API v1 endpoint group
var apiV1 = app.MapGroup("/api/v1");

// Map endpoint modules
apiV1.MapUserActionsEndpoints();

app.Run();

// Make the implicit Program class accessible to tests
public partial class Program
{
}