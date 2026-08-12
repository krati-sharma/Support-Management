using SupportTicket.Domain.Enums;

namespace SupportTicket.Application.DTOs;

public record UserDto(int Id, string Name, string Email, string Role);

public record UserSummaryDto(int Id, string Name);

public record UserDetailSummaryDto(int Id, string Name, string Email);

public record TicketListItemDto(
    int Id,
    string Title,
    string Description,
    string Priority,
    string Status,
    UserSummaryDto AssignedTo,
    UserSummaryDto CreatedBy,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int CommentCount);

public record CommentDto(
    int Id,
    string Message,
    UserSummaryDto CreatedBy,
    DateTime CreatedAt);

public record TicketDetailDto(
    int Id,
    string Title,
    string Description,
    string Priority,
    string Status,
    UserDetailSummaryDto AssignedTo,
    UserDetailSummaryDto CreatedBy,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<CommentDto> Comments,
    IReadOnlyList<string> ValidNextStatuses);

public record CreateTicketRequest(
    string Title,
    string Description,
    Priority? Priority,
    int CreatedById,
    int AssignedToId);

public record UpdateTicketRequest(
    string Title,
    string Description,
    Priority? Priority,
    int AssignedToId);

public record ChangeStatusRequest(TicketStatus? Status);

public record CreateCommentRequest(string Message, int CreatedById);

public record ValidTransitionsDto(string CurrentStatus, IReadOnlyList<string> ValidNextStatuses);
