using Microsoft.EntityFrameworkCore;
using SupportTicket.Domain.Entities;
using SupportTicket.Domain.Enums;
using SupportTicket.Infrastructure.Persistence;

namespace SupportTicket.Infrastructure.Persistence.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        await SeedUsersAsync(context, cancellationToken);
        await SeedTicketsAndCommentsAsync(context, cancellationToken);
    }

    private static async Task SeedUsersAsync(AppDbContext context, CancellationToken cancellationToken)
    {
        var users = new[]
        {
            new User { Name = "Alice Agent", Email = "alice.agent@example.com", Role = UserRole.Agent },
            new User { Name = "Bob Supervisor", Email = "bob.supervisor@example.com", Role = UserRole.Supervisor },
            new User { Name = "Carol Admin", Email = "carol.admin@example.com", Role = UserRole.Admin },
            new User { Name = "Dave Agent", Email = "dave.agent@example.com", Role = UserRole.Agent }
        };

        foreach (var user in users)
        {
            if (!await context.Users.AnyAsync(u => u.Email == user.Email, cancellationToken))
            {
                context.Users.Add(user);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedTicketsAndCommentsAsync(AppDbContext context, CancellationToken cancellationToken)
    {
        var alice = await context.Users.SingleAsync(u => u.Email == "alice.agent@example.com", cancellationToken);
        var bob = await context.Users.SingleAsync(u => u.Email == "bob.supervisor@example.com", cancellationToken);
        var carol = await context.Users.SingleAsync(u => u.Email == "carol.admin@example.com", cancellationToken);
        var dave = await context.Users.SingleAsync(u => u.Email == "dave.agent@example.com", cancellationToken);

        var baseTime = new DateTime(2026, 8, 11, 8, 0, 0, DateTimeKind.Utc);

        await EnsureTicketAsync(
            context,
            "Cannot login to portal",
            "User reports 401 after password reset.",
            Priority.High,
            TicketStatus.Open,
            alice.Id,
            dave.Id,
            baseTime,
            cancellationToken,
            (dave.Id, "Reset link expired — sending new one.", baseTime.AddMinutes(15)),
            (alice.Id, "Waiting for user confirmation.", baseTime.AddMinutes(45)));

        await EnsureTicketAsync(
            context,
            "Printer not working",
            "Office printer on floor 2 shows offline.",
            Priority.Medium,
            TicketStatus.InProgress,
            bob.Id,
            alice.Id,
            baseTime.AddHours(1),
            cancellationToken,
            (alice.Id, "Checked network — printer appears offline.", baseTime.AddHours(1).AddMinutes(20)));

        await EnsureTicketAsync(
            context,
            "Email sync delay",
            "Outlook sync takes several minutes.",
            Priority.Low,
            TicketStatus.Resolved,
            carol.Id,
            dave.Id,
            baseTime.AddHours(2),
            cancellationToken,
            (dave.Id, "Increased sync interval on mailbox.", baseTime.AddHours(2).AddMinutes(10)),
            (carol.Id, "Issue confirmed resolved by requester.", baseTime.AddHours(2).AddMinutes(40)));

        await EnsureTicketAsync(
            context,
            "VPN connection drops",
            "VPN disconnects every 10 minutes.",
            Priority.High,
            TicketStatus.Closed,
            alice.Id,
            bob.Id,
            baseTime.AddHours(3),
            cancellationToken);

        await EnsureTicketAsync(
            context,
            "Request new monitor",
            "Requesting a second monitor for dual-screen setup.",
            Priority.Low,
            TicketStatus.Cancelled,
            dave.Id,
            alice.Id,
            baseTime.AddHours(4),
            cancellationToken);
    }

    private static async Task EnsureTicketAsync(
        AppDbContext context,
        string title,
        string description,
        Priority priority,
        TicketStatus status,
        int createdById,
        int assignedToId,
        DateTime createdAt,
        CancellationToken cancellationToken,
        params (int CreatedById, string Message, DateTime CreatedAt)[] comments)
    {
        if (await context.Tickets.AnyAsync(t => t.Title == title, cancellationToken))
        {
            return;
        }

        var ticket = new Ticket
        {
            Title = title,
            Description = description,
            Priority = priority,
            Status = status,
            CreatedById = createdById,
            AssignedToId = assignedToId,
            CreatedAt = createdAt,
            UpdatedAt = createdAt.AddMinutes(30)
        };

        foreach (var comment in comments)
        {
            ticket.Comments.Add(new Comment
            {
                Message = comment.Message,
                CreatedById = comment.CreatedById,
                CreatedAt = comment.CreatedAt
            });
        }

        context.Tickets.Add(ticket);
        await context.SaveChangesAsync(cancellationToken);
    }
}
