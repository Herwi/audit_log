namespace AuditLog.Application.Dtos;

/// <summary>
/// Generic wrapper for paginated API responses
/// </summary>
/// <typeparam name="T">The type of data being paginated</typeparam>
public class PagedResponse<T>
{
    public required IReadOnlyList<T> Data { get; init; }
    public required PaginationMetadata Pagination { get; init; }
}

/// <summary>
/// Metadata about the pagination state
/// </summary>
public class PaginationMetadata
{
    public required int CurrentPage { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required int TotalPages { get; init; }
    public required bool HasPreviousPage { get; init; }
    public required bool HasNextPage { get; init; }
}
