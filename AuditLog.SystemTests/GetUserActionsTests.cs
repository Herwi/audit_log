using System.Net;
using System.Net.Http.Json;
using AuditLog.Domain.Entities;

namespace AuditLog.SystemTests;

[Collection(AuditLogWebApplicationFixture.CollectionName)]
public class GetUserActionsTests : IClassFixture<AuditLogWebApplicationFixture>
{
    private readonly HttpClient _client;
    private readonly AuditLogWebApplicationFixture _fixture;
    private static readonly Guid TestOrganizationId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

    public GetUserActionsTests(AuditLogWebApplicationFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task GetUserActions_ReturnsFirstPage_With10Items()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions?page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actions = await response.Content.ReadFromJsonAsync<List<UserAction>>();
        Assert.NotNull(actions);
        Assert.Equal(10, actions.Count);
    }

    [Fact]
    public async Task GetUserActions_ReturnsSecondPage_With5Items()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions?page=2&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actions = await response.Content.ReadFromJsonAsync<List<UserAction>>();
        Assert.NotNull(actions);
        Assert.Equal(5, actions.Count);
    }

    [Fact]
    public async Task GetUserActions_ReturnsSecondPage_WhenPageSizeIs5()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions?page=2&pageSize=5");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actions = await response.Content.ReadFromJsonAsync<List<UserAction>>();
        Assert.NotNull(actions);
        Assert.Equal(5, actions.Count);
    }

    [Fact]
    public async Task GetUserActions_ReturnsSortedByDateDescending()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions?page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actions = await response.Content.ReadFromJsonAsync<List<UserAction>>();
        Assert.NotNull(actions);
        Assert.Equal(10, actions.Count);

        // Verify descending order (newest first)
        for (int i = 0; i < actions.Count - 1; i++)
        {
            Assert.True(actions[i].StartDate >= actions[i + 1].StartDate,
                $"Actions should be sorted by date descending. Action at index {i} has StartDate {actions[i].StartDate}, " +
                $"but action at index {i + 1} has StartDate {actions[i + 1].StartDate}");
        }
    }

    [Fact]
    public async Task GetUserActions_ReturnsEmptyList_ForDifferentOrganization()
    {
        // Arrange
        var differentOrgId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{differentOrgId}/user-actions?page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actions = await response.Content.ReadFromJsonAsync<List<UserAction>>();
        Assert.NotNull(actions);
        Assert.Empty(actions);
    }

    [Fact]
    public async Task GetUserActions_IncludesContractNumber_ForContractActions()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions?page=1&pageSize=15");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actions = await response.Content.ReadFromJsonAsync<List<UserAction>>();
        Assert.NotNull(actions);
        Assert.Equal(15, actions.Count);

        // Actions with ContractHeaderEntity should have contract numbers
        // Action 1 (ContractCreated) - should have contract number 2024/01/001
        var contractCreatedAction1 = actions.FirstOrDefault(a =>
            a.ActionType == ActionType.ContractCreated &&
            a.UserEmail == "jan.kowalski@company.pl");
        Assert.NotNull(contractCreatedAction1);
        Assert.Equal("2024/01/001", contractCreatedAction1.ContractNumber);

        // Action 3 (ContractModified) - should have contract number 2024/01/001
        var contractModifiedAction = actions.FirstOrDefault(a =>
            a.ActionType == ActionType.ContractModified &&
            a.UserEmail == "piotr.wisniewski@company.pl");
        Assert.NotNull(contractModifiedAction);
        Assert.Equal("2024/01/001", contractModifiedAction.ContractNumber);

        // Action 8 (ContractCreated) - should have contract number 2024/01/002
        var contractCreatedAction2 = actions.FirstOrDefault(a =>
            a.ActionType == ActionType.ContractCreated &&
            a.UserEmail == "robert.kaminski@company.pl");
        Assert.NotNull(contractCreatedAction2);
        Assert.Equal("2024/01/002", contractCreatedAction2.ContractNumber);

        // Action 11 (ContractCreated) - should have contract number 2024/01/003
        var contractCreatedAction3 = actions.FirstOrDefault(a =>
            a.ActionType == ActionType.ContractCreated &&
            a.UserEmail == "katarzyna.nowak@company.pl");
        Assert.NotNull(contractCreatedAction3);
        Assert.Equal("2024/01/003", contractCreatedAction3.ContractNumber);

        // Action 14 (ContractModified) - should have contract number 2024/01/003
        var contractModifiedAction2 = actions.FirstOrDefault(a =>
            a.ActionType == ActionType.ContractModified &&
            a.UserEmail == "lukasz.mazur@company.pl");
        Assert.NotNull(contractModifiedAction2);
        Assert.Equal("2024/01/003", contractModifiedAction2.ContractNumber);

        // Non-contract actions should NOT have contract numbers
        var annexAction = actions.FirstOrDefault(a => a.ActionType == ActionType.AnnexCreated);
        Assert.NotNull(annexAction);
        Assert.Null(annexAction.ContractNumber);

        var invoiceAction = actions.FirstOrDefault(a => a.ActionType == ActionType.InvoiceCreated);
        Assert.NotNull(invoiceAction);
        Assert.Null(invoiceAction.ContractNumber);
    }

    [Fact]
    public async Task GetUserActions_CalculatesDurationCorrectly()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions?page=1&pageSize=15");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actions = await response.Content.ReadFromJsonAsync<List<UserAction>>();
        Assert.NotNull(actions);
        Assert.Equal(15, actions.Count);

        foreach (var action in actions)
        {
            // Duration should be EndDate - StartDate
            var expectedDuration = action.EndDate - action.StartDate;
            Assert.Equal(expectedDuration, action.Duration);

            // EndDate should be >= StartDate
            Assert.True(action.EndDate >= action.StartDate,
                $"EndDate ({action.EndDate}) should be >= StartDate ({action.StartDate}) for action with correlation_id");
        }
    }

    [Fact]
    public async Task GetUserActions_CountsChangedEntitiesCorrectly()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions?page=1&pageSize=15");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actions = await response.Content.ReadFromJsonAsync<List<UserAction>>();
        Assert.NotNull(actions);
        Assert.Equal(15, actions.Count);

        // Verify specific action counts based on seed data
        // Action 1: 3 entries
        var action1 = actions.FirstOrDefault(a => a.UserEmail == "jan.kowalski@company.pl" && a.ActionType == ActionType.ContractCreated);
        Assert.NotNull(action1);
        Assert.Equal(3, action1.ChangedEntitiesCount);

        // Action 2: 2 entries
        var action2 = actions.FirstOrDefault(a => a.UserEmail == "anna.nowak@company.pl");
        Assert.NotNull(action2);
        Assert.Equal(2, action2.ChangedEntitiesCount);

        // Action 4: 4 entries (InvoiceCreated)
        var action4 = actions.FirstOrDefault(a => a.UserEmail == "maria.dabrowska@company.pl");
        Assert.NotNull(action4);
        Assert.Equal(4, action4.ChangedEntitiesCount);

        // Action 8: 6 entries (Complex contract creation)
        var action8 = actions.FirstOrDefault(a => a.UserEmail == "robert.kaminski@company.pl");
        Assert.NotNull(action8);
        Assert.Equal(6, action8.ChangedEntitiesCount);
    }

    [Fact]
    public async Task GetUserActions_ReturnsCorrectUserEmails()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions?page=1&pageSize=15");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actions = await response.Content.ReadFromJsonAsync<List<UserAction>>();
        Assert.NotNull(actions);
        Assert.Equal(15, actions.Count);

        // Verify all user emails are populated
        Assert.All(actions, action => Assert.False(string.IsNullOrEmpty(action.UserEmail)));

        // Verify specific emails from seed data
        Assert.Contains(actions, a => a.UserEmail == "jan.kowalski@company.pl");
        Assert.Contains(actions, a => a.UserEmail == "anna.nowak@company.pl");
        Assert.Contains(actions, a => a.UserEmail == "piotr.wisniewski@company.pl");
    }

    [Fact]
    public async Task GetUserActions_ReturnsCorrectActionTypes()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions?page=1&pageSize=15");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actions = await response.Content.ReadFromJsonAsync<List<UserAction>>();
        Assert.NotNull(actions);
        Assert.Equal(15, actions.Count);

        // Verify action types are determined from first audit log entry
        var contractCreatedActions = actions.Where(a => a.ActionType == ActionType.ContractCreated).ToList();
        Assert.NotEmpty(contractCreatedActions);
        Assert.Equal(3, contractCreatedActions.Count); // Actions 1, 8, 11

        var annexCreatedActions = actions.Where(a => a.ActionType == ActionType.AnnexCreated).ToList();
        Assert.NotEmpty(annexCreatedActions);
        Assert.Equal(3, annexCreatedActions.Count); // Actions 2, 10, 12

        var contractModifiedActions = actions.Where(a => a.ActionType == ActionType.ContractModified).ToList();
        Assert.NotEmpty(contractModifiedActions);
        Assert.Equal(2, contractModifiedActions.Count); // Actions 3, 14
    }

    [Fact]
    public async Task GetUserActions_ReturnsCorrectOrganizationId()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions?page=1&pageSize=15");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actions = await response.Content.ReadFromJsonAsync<List<UserAction>>();
        Assert.NotNull(actions);
        Assert.Equal(15, actions.Count);

        // All actions should have the same organization ID
        Assert.All(actions, action => Assert.Equal(TestOrganizationId, action.OrganizationId));
    }

    [Fact]
    public async Task GetUserActions_DefaultsToPage1AndPageSize10_WhenNotSpecified()
    {
        // Act - no query parameters
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actions = await response.Content.ReadFromJsonAsync<List<UserAction>>();
        Assert.NotNull(actions);
        Assert.Equal(10, actions.Count); // Default page size is 10
    }
}
