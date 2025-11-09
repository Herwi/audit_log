using System.Net;
using System.Net.Http.Json;

namespace AuditLog.SystemTests;

public class GetUserActivitiesTests : IClassFixture<AuditLogWebApplicationFixture>
{
    private readonly HttpClient _client;
    private readonly AuditLogWebApplicationFixture _fixture;

    public GetUserActivitiesTests(AuditLogWebApplicationFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task GetUserActivities_ReturnsEmptyList_WhenNoActivitiesExist()
    {
        // Arrange
        const string organizationId = "test-org-123";

        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{organizationId}/user-activities");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var activities = await response.Content.ReadFromJsonAsync<List<UserActivity>>();
        Assert.NotNull(activities);
        Assert.Empty(activities);
    }

    [Fact]
    public async Task GetUserActivities_ReturnsOk_ForDifferentOrganizationIds()
    {
        // Arrange
        var organizationIds = new[] { "org-1", "org-2", "org-abc" };

        foreach (var orgId in organizationIds)
        {
            // Act
            var response = await _client.GetAsync($"/api/v1/organizations/{orgId}/user-activities");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var activities = await response.Content.ReadFromJsonAsync<List<UserActivity>>();
            Assert.NotNull(activities);
        }
    }
}

// This record needs to match the one in Program.cs
// In a real application, you'd likely share this from a common project
public record UserActivity;