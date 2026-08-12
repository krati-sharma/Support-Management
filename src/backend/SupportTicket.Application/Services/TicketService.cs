using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SupportTicket.Application.DTOs;
using SupportTicket.Application.Interfaces;
using SupportTicket.Domain.Entities;
using SupportTicket.Domain.Enums;
using SupportTicket.Domain.Exceptions;
using ValidationException = SupportTicket.Domain.Exceptions.ValidationException;

namespace SupportTicket.Application.Services;

public class TicketService : ITicketService
{
    private readonly IAppDbContext _db;
    private readonly IStatusTransitionService _statusTransitions;
    private readonly IValidator<CreateTicketRequest> _createValidator;
    private readonly IValidator<UpdateTicketRequest> _updateValidator;
    private readonly IValidator<CreateCommentRequest> _commentValidator;
    private readonly IValidator<ChangeStatusRequest> _statusValidator;

    public TicketService(
        IAppDbContext db,
        IStatusTransitionService statusTransitions,
        IValidator<CreateTicketRequest> createValidator,
        IValidator<UpdateTicketRequest> updateValidator,
        IValidator<CreateCommentRequest> commentValidator,
        IValidator<ChangeStatusRequest> statusValidator)
    {
        _db = db;
        _statusTransitions = statusTransitions;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _commentValidator = commentValidator;
        _statusValidator = statusValidator;
    }

    public async Task<IReadOnlyList<TicketListItemDto>> GetTicketsAsync(
        string? search,
        TicketStatus? status,
        CancellationToken cancellationToken = default)
    {
        // Projection below loads related fields; Include is unnecessary with Select.
        var query = _db.Tickets.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(t =>
                t.Title.Contains(term) ||
                t.Description.Contains(term));
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        var tickets = await query
            .OrderByDescending(t => t.UpdatedAt)
            .Select(t => new TicketListItemDto(
                t.Id,
                t.Title,
                t.Description,
                t.Priority.ToString(),
                t.Status.ToString(),
                new UserSummaryDto(t.AssignedTo.Id, t.AssignedTo.Name),
                new UserSummaryDto(t.CreatedBy.Id, t.CreatedBy.Name),
                t.CreatedAt,
                t.UpdatedAt,
                t.Comments.Count))
            .ToListAsync(cancellationToken);

        return tickets;
    }

    public async Task<TicketDetailDto> GetTicketByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var ticket = await LoadTicketDetailAsync(id, cancellationToken);
        return MapDetail(ticket);
    }

    public async Task<TicketDetailDto> CreateTicketAsync(CreateTicketRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createValidator, request, cancellationToken);
        await EnsureUsersExistAsync(request.CreatedById, request.AssignedToId, cancellationToken);

        var now = DateTime.UtcNow;
        var ticket = new Ticket
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Priority = request.Priority!.Value,
            Status = TicketStatus.Open,
            CreatedById = request.CreatedById,
            AssignedToId = request.AssignedToId,
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync(cancellationToken);

        return await GetTicketByIdAsync(ticket.Id, cancellationToken);
    }

    public async Task<TicketDetailDto> UpdateTicketAsync(int id, UpdateTicketRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_updateValidator, request, cancellationToken);

        var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Ticket with id {id} was not found.");

        await EnsureUsersExistAsync(null, request.AssignedToId, cancellationToken);

        ticket.Title = request.Title.Trim();
        ticket.Description = request.Description.Trim();
        ticket.Priority = request.Priority!.Value;
        ticket.AssignedToId = request.AssignedToId;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return await GetTicketByIdAsync(id, cancellationToken);
    }

    public async Task<TicketDetailDto> ChangeStatusAsync(int id, ChangeStatusRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_statusValidator, request, cancellationToken);

        var ticket = await _db.Tickets.FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Ticket with id {id} was not found.");

        _statusTransitions.EnsureValidTransition(ticket.Status, request.Status!.Value);

        ticket.Status = request.Status.Value;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return await GetTicketByIdAsync(id, cancellationToken);
    }

    public async Task<ValidTransitionsDto> GetValidTransitionsAsync(int id, CancellationToken cancellationToken = default)
    {
        var ticket = await _db.Tickets.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Ticket with id {id} was not found.");

        var next = _statusTransitions.GetValidNextStatuses(ticket.Status)
            .Select(s => s.ToString())
            .ToList();

        return new ValidTransitionsDto(ticket.Status.ToString(), next);
    }

    public async Task<CommentDto> AddCommentAsync(int ticketId, CreateCommentRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_commentValidator, request, cancellationToken);

        var ticketExists = await _db.Tickets.AnyAsync(t => t.Id == ticketId, cancellationToken);
        if (!ticketExists)
        {
            throw new NotFoundException($"Ticket with id {ticketId} was not found.");
        }

        await EnsureUsersExistAsync(request.CreatedById, null, cancellationToken);

        var comment = new Comment
        {
            TicketId = ticketId,
            Message = request.Message.Trim(),
            CreatedById = request.CreatedById,
            CreatedAt = DateTime.UtcNow
        };

        _db.Comments.Add(comment);

        var ticket = await _db.Tickets.FirstAsync(t => t.Id == ticketId, cancellationToken);
        ticket.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        var created = await _db.Comments
            .AsNoTracking()
            .Include(c => c.CreatedBy)
            .FirstAsync(c => c.Id == comment.Id, cancellationToken);

        return new CommentDto(
            created.Id,
            created.Message,
            new UserSummaryDto(created.CreatedBy.Id, created.CreatedBy.Name),
            created.CreatedAt);
    }

    private async Task<Ticket> LoadTicketDetailAsync(int id, CancellationToken cancellationToken)
    {
        var ticket = await _db.Tickets
            .AsNoTracking()
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .Include(t => t.Comments)
                .ThenInclude(c => c.CreatedBy)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return ticket ?? throw new NotFoundException($"Ticket with id {id} was not found.");
    }

    private TicketDetailDto MapDetail(Ticket ticket)
    {
        var comments = ticket.Comments
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentDto(
                c.Id,
                c.Message,
                new UserSummaryDto(c.CreatedBy.Id, c.CreatedBy.Name),
                c.CreatedAt))
            .ToList();

        var validNext = _statusTransitions.GetValidNextStatuses(ticket.Status)
            .Select(s => s.ToString())
            .ToList();

        return new TicketDetailDto(
            ticket.Id,
            ticket.Title,
            ticket.Description,
            ticket.Priority.ToString(),
            ticket.Status.ToString(),
            new UserDetailSummaryDto(ticket.AssignedTo.Id, ticket.AssignedTo.Name, ticket.AssignedTo.Email),
            new UserDetailSummaryDto(ticket.CreatedBy.Id, ticket.CreatedBy.Name, ticket.CreatedBy.Email),
            ticket.CreatedAt,
            ticket.UpdatedAt,
            comments,
            validNext);
    }

    private async Task EnsureUsersExistAsync(int? createdById, int? assignedToId, CancellationToken cancellationToken)
    {
        if (createdById.HasValue)
        {
            var exists = await _db.Users.AnyAsync(u => u.Id == createdById.Value, cancellationToken);
            if (!exists)
            {
                throw new ValidationException("createdById", $"User with id {createdById.Value} was not found.");
            }
        }

        if (assignedToId.HasValue)
        {
            var exists = await _db.Users.AnyAsync(u => u.Id == assignedToId.Value, cancellationToken);
            if (!exists)
            {
                throw new ValidationException("assignedToId", $"User with id {assignedToId.Value} was not found.");
            }
        }
    }

    private static async Task ValidateAsync<T>(IValidator<T> validator, T request, CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            var errors = result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            throw new ValidationException(errors);
        }
    }
}
