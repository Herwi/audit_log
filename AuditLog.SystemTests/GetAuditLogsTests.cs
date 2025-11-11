using System.Net;
using System.Net.Http.Json;
using AuditLog.Domain.Entities;

namespace AuditLog.SystemTests;

[Collection(AuditLogWebApplicationFixture.CollectionName)]
public class GetAuditLogsTests : IClassFixture<AuditLogWebApplicationFixture>
{
    private readonly HttpClient _client;
    private readonly AuditLogWebApplicationFixture _fixture;
    private static readonly Guid TestOrganizationId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

    public GetAuditLogsTests(AuditLogWebApplicationFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task GetAuditLogs_ReturnsAllEntriesForCorrelationId_InChronologicalOrder()
    {
        // Arrange - ACTION 1 has 3 audit log entries
        var correlationId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions/{correlationId}/audit-logs");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auditLogs = await response.Content.ReadFromJsonAsync<List<AuditLogEntry>>();
        Assert.NotNull(auditLogs);
        Assert.Equal(3, auditLogs.Count);

        // Verify chronological order
        for (int i = 0; i < auditLogs.Count - 1; i++)
        {
            Assert.True(auditLogs[i].CreatedDate <= auditLogs[i + 1].CreatedDate,
                $"Audit logs should be sorted chronologically. Entry at index {i} has CreatedDate {auditLogs[i].CreatedDate}, " +
                $"but entry at index {i + 1} has CreatedDate {auditLogs[i + 1].CreatedDate}");
        }

        // Verify all entries have the same correlation ID
        Assert.All(auditLogs, log => Assert.Equal(correlationId, log.CorrelationId));
    }

    [Fact]
    public async Task GetAuditLogs_ReturnsEmptyList_ForNonExistentCorrelationId()
    {
        // Arrange
        var nonExistentCorrelationId = Guid.Parse("00000000-0000-0000-0000-000000000000");

        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions/{nonExistentCorrelationId}/audit-logs");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auditLogs = await response.Content.ReadFromJsonAsync<List<AuditLogEntry>>();
        Assert.NotNull(auditLogs);
        Assert.Empty(auditLogs);
    }

    [Fact]
    public async Task GetAuditLogs_ReturnsAllRequiredFields_ForEachEntry()
    {
        // Arrange - ACTION 1 has 3 audit log entries
        var correlationId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions/{correlationId}/audit-logs");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auditLogs = await response.Content.ReadFromJsonAsync<List<AuditLogEntry>>();
        Assert.NotNull(auditLogs);
        Assert.NotEmpty(auditLogs);

        // Verify all required fields are populated for each entry
        foreach (var log in auditLogs)
        {
            Assert.NotNull(log.UserEmail);
            Assert.NotEqual(default, log.OperationType);
            Assert.NotEqual(default, log.EntityType);
            Assert.NotEqual(default, log.CreatedDate);
            Assert.NotEqual(Guid.Empty, log.CorrelationId);
            Assert.NotEqual(Guid.Empty, log.EntityId);
        }

        // Verify first entry specific values (ACTION 1 first entry)
        var firstEntry = auditLogs[0];
        Assert.Equal("jan.kowalski@company.pl", firstEntry.UserEmail);
        Assert.Equal(Infrastructure.Persistence.Entities.OperationType.Added, firstEntry.OperationType);
        Assert.Equal(Infrastructure.Persistence.Entities.EntityType.ContractHeaderEntity, firstEntry.EntityType);
    }
}
