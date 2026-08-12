using SupportTicket.Domain.Enums;
using SupportTicket.Domain.Exceptions;

namespace SupportTicket.Application.Services;

public class StatusTransitionService : Interfaces.IStatusTransitionService
{
    private static readonly IReadOnlyDictionary<TicketStatus, HashSet<TicketStatus>> AllowedTransitions =
        new Dictionary<TicketStatus, HashSet<TicketStatus>>
        {
            [TicketStatus.Open] = new HashSet<TicketStatus> { TicketStatus.InProgress, TicketStatus.Cancelled },
            [TicketStatus.InProgress] = new HashSet<TicketStatus> { TicketStatus.Resolved, TicketStatus.Cancelled },
            [TicketStatus.Resolved] = new HashSet<TicketStatus> { TicketStatus.Closed },
            [TicketStatus.Closed] = new HashSet<TicketStatus>(),
            [TicketStatus.Cancelled] = new HashSet<TicketStatus>()
        };

    public bool CanTransition(TicketStatus from, TicketStatus to)
    {
        return AllowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
    }

    public IReadOnlyCollection<TicketStatus> GetValidNextStatuses(TicketStatus current)
    {
        return AllowedTransitions.TryGetValue(current, out var allowed)
            ? allowed.OrderBy(s => s.ToString()).ToList().AsReadOnly()
            : Array.Empty<TicketStatus>();
    }

    public void EnsureValidTransition(TicketStatus from, TicketStatus to)
    {
        if (from == to)
        {
            throw new InvalidStatusTransitionException(from, to, GetValidNextStatuses(from));
        }

        if (!CanTransition(from, to))
        {
            throw new InvalidStatusTransitionException(from, to, GetValidNextStatuses(from));
        }
    }
}
