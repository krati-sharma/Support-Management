using SupportTicket.Domain.Enums;

namespace SupportTicket.Domain.Exceptions;

public class InvalidStatusTransitionException : Exception
{
    public TicketStatus FromStatus { get; }
    public TicketStatus ToStatus { get; }
    public IReadOnlyCollection<TicketStatus> AllowedStatuses { get; }

    public InvalidStatusTransitionException(
        TicketStatus fromStatus,
        TicketStatus toStatus,
        IEnumerable<TicketStatus> allowedStatuses)
        : base(BuildMessage(fromStatus, toStatus, allowedStatuses))
    {
        FromStatus = fromStatus;
        ToStatus = toStatus;
        AllowedStatuses = allowedStatuses.ToList().AsReadOnly();
    }

    private static string BuildMessage(
        TicketStatus fromStatus,
        TicketStatus toStatus,
        IEnumerable<TicketStatus> allowedStatuses)
    {
        var allowed = allowedStatuses.ToList();
        var allowedText = allowed.Count == 0
            ? "none (terminal state)"
            : string.Join(", ", allowed);

        return $"Cannot transition from '{fromStatus}' to '{toStatus}'. Allowed transitions from '{fromStatus}': {allowedText}.";
    }
}
