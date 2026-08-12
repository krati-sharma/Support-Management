using Microsoft.EntityFrameworkCore;
using SupportTicket.Application.DTOs;
using SupportTicket.Application.Interfaces;

namespace SupportTicket.Application.Services;

public class UserService : IUserService
{
    private readonly IAppDbContext _db;

    public UserService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Users
            .AsNoTracking()
            .OrderBy(u => u.Name)
            .Select(u => new UserDto(u.Id, u.Name, u.Email, u.Role.ToString()))
            .ToListAsync(cancellationToken);
    }
}
