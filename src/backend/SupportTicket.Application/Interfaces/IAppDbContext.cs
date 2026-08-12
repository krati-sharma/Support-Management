using SupportTicket.Domain.Enums;

namespace SupportTicket.Application.Interfaces;

public interface IAppDbContext
{
    Microsoft.EntityFrameworkCore.DbSet<Domain.Entities.User> Users { get; }
    Microsoft.EntityFrameworkCore.DbSet<Domain.Entities.Ticket> Tickets { get; }
    Microsoft.EntityFrameworkCore.DbSet<Domain.Entities.Comment> Comments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
