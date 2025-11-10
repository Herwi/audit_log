using System.Net;
using System.Net.Http.Json;
using AuditLog.Domain.Entities;

namespace AuditLog.SystemTests;

public class GetUserActionsTests : IClassFixture<AuditLogWebApplicationFixture>
{
    private readonly HttpClient _client;
    private readonly AuditLogWebApplicationFixture _fixture;

    public GetUserActionsTests(AuditLogWebApplicationFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task GetUserActions_ReturnsEmptyList_WhenNoActionsExist()
    {
        // Arrange
        const string organizationId = "test-org-123";

        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{organizationId}/user-actions");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var actions = await response.Content.ReadFromJsonAsync<List<UserAction>>();
        Assert.NotNull(actions);
        Assert.Empty(actions);
    }

    [Fact]
    public async Task GetUserActions_ReturnsOk_ForDifferentOrganizationIds()
    {
        // Arrange
        var organizationIds = new[] { "org-1", "org-2", "org-abc" };

        foreach (var orgId in organizationIds)
        {
            // Act
            var response = await _client.GetAsync($"/api/v1/organizations/{orgId}/user-actions");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var actions = await response.Content.ReadFromJsonAsync<List<UserAction>>();
            Assert.NotNull(actions);
        }
    }
}
