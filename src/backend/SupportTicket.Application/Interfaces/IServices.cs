using SupportTicket.Application.DTOs;
using SupportTicket.Domain.Enums;

namespace SupportTicket.Application.Interfaces;

public interface ITicketService
{
    Task<IReadOnlyList<TicketListItemDto>> GetTicketsAsync(string? search, TicketStatus? status, CancellationToken cancellationToken = default);
    Task<TicketDetailDto> GetTicketByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TicketDetailDto> CreateTicketAsync(CreateTicketRequest request, CancellationToken cancellationToken = default);
    Task<TicketDetailDto> UpdateTicketAsync(int id, UpdateTicketRequest request, CancellationToken cancellationToken = default);
    Task<TicketDetailDto> ChangeStatusAsync(int id, ChangeStatusRequest request, CancellationToken cancellationToken = default);
    Task<ValidTransitionsDto> GetValidTransitionsAsync(int id, CancellationToken cancellationToken = default);
    Task<CommentDto> AddCommentAsync(int ticketId, CreateCommentRequest request, CancellationToken cancellationToken = default);
}

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default);
}

public interface IStatusTransitionService
{
    bool CanTransition(TicketStatus from, TicketStatus to);
    IReadOnlyCollection<TicketStatus> GetValidNextStatuses(TicketStatus current);
    void EnsureValidTransition(TicketStatus from, TicketStatus to);
}
