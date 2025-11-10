using System.Net;
using System.Net.Http.Json;

namespace AuditLog.SystemTests;

[Collection(AuditLogWebApplicationFixture.CollectionName)]
public class GetOrganizationsTests : IClassFixture<AuditLogWebApplicationFixture>
{
    private readonly HttpClient _client;
    private static readonly Guid Organization1Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
    private static readonly Guid Organization2Id = Guid.Parse("7b91c8e2-4d3a-4f1e-9c8b-5a6d7e8f9a0b");
    private static readonly Guid Organization3Id = Guid.Parse("9d2e4f6a-8b1c-4e5d-a7f9-3c5b8d1e4a7b");

    public GetOrganizationsTests(AuditLogWebApplicationFixture fixture)
    {
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task GetOrganizations_ReturnsDistinctOrganizationIdsFromSeedData()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/organizations");

        // Assert - HTTP 200
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var organizationIds = await response.Content.ReadFromJsonAsync<List<Guid>>();
        Assert.NotNull(organizationIds);

        // Assert - Exactly 3 distinct organizations
        Assert.Equal(3, organizationIds.Count);

        // Assert - Contains all expected organization IDs from seed data
        Assert.Contains(Organization1Id, organizationIds);
        Assert.Contains(Organization2Id, organizationIds);
        Assert.Contains(Organization3Id, organizationIds);

        // Assert - Sorted order for consistent behavior
        var sortedIds = organizationIds.OrderBy(id => id).ToList();
        Assert.Equal(sortedIds, organizationIds);
    }
}
