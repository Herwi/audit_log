using System.Net;
using System.Net.Http.Json;
using AuditLog.Application.Dtos;
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
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<UserAction>>();
        Assert.NotNull(pagedResponse);
        Assert.Equal(10, pagedResponse.Data.Count);
        Assert.Equal(1, pagedResponse.Pagination.CurrentPage);
        Assert.Equal(10, pagedResponse.Pagination.PageSize);
    }

    [Fact]
    public async Task GetUserActions_ReturnsSecondPage_With5Items()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions?page=2&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<UserAction>>();
        Assert.NotNull(pagedResponse);
        Assert.Equal(5, pagedResponse.Data.Count);
        Assert.Equal(2, pagedResponse.Pagination.CurrentPage);
        Assert.Equal(10, pagedResponse.Pagination.PageSize);
    }

    [Fact]
    public async Task GetUserActions_ReturnsSecondPage_WhenPageSizeIs5()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions?page=2&pageSize=5");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<UserAction>>();
        Assert.NotNull(pagedResponse);
        Assert.Equal(5, pagedResponse.Data.Count);
    }

    [Fact]
    public async Task GetUserActions_ReturnsSortedByDateDescending()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions?page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<UserAction>>();
        Assert.NotNull(pagedResponse);
        Assert.Equal(10, pagedResponse.Data.Count);

        // Verify descending order (newest first)
        for (int i = 0; i < pagedResponse.Data.Count - 1; i++)
        {
            Assert.True(pagedResponse.Data[i].StartDate >= pagedResponse.Data[i + 1].StartDate,
                $"Actions should be sorted by date descending. Action at index {i} has StartDate {pagedResponse.Data[i].StartDate}, " +
                $"but action at index {i + 1} has StartDate {pagedResponse.Data[i + 1].StartDate}");
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
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<UserAction>>();
        Assert.NotNull(pagedResponse);
        Assert.Empty(pagedResponse.Data);
    }

    [Fact]
    public async Task GetUserActions_IncludesContractNumber_ForContractActions()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions?page=1&pageSize=15");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<UserAction>>();
        Assert.NotNull(pagedResponse);
        Assert.Equal(15, pagedResponse.Data.Count);

        // Actions with ContractHeaderEntity should have contract numbers
        // Action 1 (ContractCreated) - should have contract number 2024/01/001
        var contractCreatedAction1 = pagedResponse.Data.FirstOrDefault(a =>
            a.ActionType == ActionType.ContractCreated &&
            a.UserEmail == "jan.kowalski@company.pl");
        Assert.NotNull(contractCreatedAction1);
        Assert.Equal("2024/01/001", contractCreatedAction1.ContractNumber);

        // Action 3 (ContractModified) - should have contract number 2024/01/001
        var contractModifiedAction = pagedResponse.Data.FirstOrDefault(a =>
            a.ActionType == ActionType.ContractModified &&
            a.UserEmail == "piotr.wisniewski@company.pl");
        Assert.NotNull(contractModifiedAction);
        Assert.Equal("2024/01/001", contractModifiedAction.ContractNumber);

        // Action 8 (ContractCreated) - should have contract number 2024/01/002
        var contractCreatedAction2 = pagedResponse.Data.FirstOrDefault(a =>
            a.ActionType == ActionType.ContractCreated &&
            a.UserEmail == "robert.kaminski@company.pl");
        Assert.NotNull(contractCreatedAction2);
        Assert.Equal("2024/01/002", contractCreatedAction2.ContractNumber);

        // Action 11 (ContractCreated) - should have contract number 2024/01/003
        var contractCreatedAction3 = pagedResponse.Data.FirstOrDefault(a =>
            a.ActionType == ActionType.ContractCreated &&
            a.UserEmail == "katarzyna.nowak@company.pl");
        Assert.NotNull(contractCreatedAction3);
        Assert.Equal("2024/01/003", contractCreatedAction3.ContractNumber);

        // Action 14 (ContractModified) - should have contract number 2024/01/003
        var contractModifiedAction2 = pagedResponse.Data.FirstOrDefault(a =>
            a.ActionType == ActionType.ContractModified &&
            a.UserEmail == "lukasz.mazur@company.pl");
        Assert.NotNull(contractModifiedAction2);
        Assert.Equal("2024/01/003", contractModifiedAction2.ContractNumber);

        // Non-contract actions should NOT have contract numbers
        var annexAction = pagedResponse.Data.FirstOrDefault(a => a.ActionType == ActionType.AnnexCreated);
        Assert.NotNull(annexAction);
        Assert.Null(annexAction.ContractNumber);

        var invoiceAction = pagedResponse.Data.FirstOrDefault(a => a.ActionType == ActionType.InvoiceCreated);
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
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<UserAction>>();
        Assert.NotNull(pagedResponse);
        Assert.Equal(15, pagedResponse.Data.Count);

        foreach (var action in pagedResponse.Data)
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
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<UserAction>>();
        Assert.NotNull(pagedResponse);
        Assert.Equal(15, pagedResponse.Data.Count);

        // Verify specific action counts based on seed data
        // Action 1: 3 entries
        var action1 = pagedResponse.Data.FirstOrDefault(a => a.UserEmail == "jan.kowalski@company.pl" && a.ActionType == ActionType.ContractCreated);
        Assert.NotNull(action1);
        Assert.Equal(3, action1.ChangedEntitiesCount);

        // Action 2: 2 entries
        var action2 = pagedResponse.Data.FirstOrDefault(a => a.UserEmail == "anna.nowak@company.pl");
        Assert.NotNull(action2);
        Assert.Equal(2, action2.ChangedEntitiesCount);

        // Action 4: 4 entries (InvoiceCreated)
        var action4 = pagedResponse.Data.FirstOrDefault(a => a.UserEmail == "maria.dabrowska@company.pl");
        Assert.NotNull(action4);
        Assert.Equal(4, action4.ChangedEntitiesCount);

        // Action 8: 6 entries (Complex contract creation)
        var action8 = pagedResponse.Data.FirstOrDefault(a => a.UserEmail == "robert.kaminski@company.pl");
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
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<UserAction>>();
        Assert.NotNull(pagedResponse);
        Assert.Equal(15, pagedResponse.Data.Count);

        // Verify all user emails are populated
        Assert.All(pagedResponse.Data, action => Assert.False(string.IsNullOrEmpty(action.UserEmail)));

        // Verify specific emails from seed data
        Assert.Contains(pagedResponse.Data, a => a.UserEmail == "jan.kowalski@company.pl");
        Assert.Contains(pagedResponse.Data, a => a.UserEmail == "anna.nowak@company.pl");
        Assert.Contains(pagedResponse.Data, a => a.UserEmail == "piotr.wisniewski@company.pl");
    }

    [Fact]
    public async Task GetUserActions_ReturnsCorrectActionTypes()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions?page=1&pageSize=15");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<UserAction>>();
        Assert.NotNull(pagedResponse);
        Assert.Equal(15, pagedResponse.Data.Count);

        // Verify action types are determined from first audit log entry
        var contractCreatedActions = pagedResponse.Data.Where(a => a.ActionType == ActionType.ContractCreated).ToList();
        Assert.NotEmpty(contractCreatedActions);
        Assert.Equal(3, contractCreatedActions.Count); // Actions 1, 8, 11

        var annexCreatedActions = pagedResponse.Data.Where(a => a.ActionType == ActionType.AnnexCreated).ToList();
        Assert.NotEmpty(annexCreatedActions);
        Assert.Equal(3, annexCreatedActions.Count); // Actions 2, 10, 12

        var contractModifiedActions = pagedResponse.Data.Where(a => a.ActionType == ActionType.ContractModified).ToList();
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
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<UserAction>>();
        Assert.NotNull(pagedResponse);
        Assert.Equal(15, pagedResponse.Data.Count);

        // All actions should have the same organization ID
        Assert.All(pagedResponse.Data, action => Assert.Equal(TestOrganizationId, action.OrganizationId));
    }

    [Fact]
    public async Task GetUserActions_DefaultsToPage1AndPageSize10_WhenNotSpecified()
    {
        // Act - no query parameters
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<UserAction>>();
        Assert.NotNull(pagedResponse);
        Assert.Equal(10, pagedResponse.Data.Count); // Default page size is 10
    }

    [Theory]
    [InlineData(1, 10, 2, false, true)]  // First page: page 1 of 2
    [InlineData(2, 10, 2, true, false)]  // Last page: page 2 of 2
    [InlineData(2, 5, 3, true, true)]    // Middle page: page 2 of 3
    [InlineData(1, 20, 1, false, false)] // Single page: page 1 of 1
    public async Task GetUserActions_ReturnsPaginationMetadata_ForDifferentPageScenarios(
        int page,
        int pageSize,
        int expectedTotalPages,
        bool expectedHasPreviousPage,
        bool expectedHasNextPage)
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/organizations/{TestOrganizationId}/user-actions?page={page}&pageSize={pageSize}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<UserAction>>();
        Assert.NotNull(pagedResponse);
        Assert.NotNull(pagedResponse.Pagination);

        // Verify pagination metadata
        Assert.Equal(page, pagedResponse.Pagination.CurrentPage);
        Assert.Equal(pageSize, pagedResponse.Pagination.PageSize);
        Assert.Equal(15, pagedResponse.Pagination.TotalCount); // 15 actions in seed data for Organization1
        Assert.Equal(expectedTotalPages, pagedResponse.Pagination.TotalPages);
        Assert.Equal(expectedHasPreviousPage, pagedResponse.Pagination.HasPreviousPage);
        Assert.Equal(expectedHasNextPage, pagedResponse.Pagination.HasNextPage);
    }
}
