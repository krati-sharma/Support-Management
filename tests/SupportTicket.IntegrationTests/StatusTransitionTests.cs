using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SupportTicket.Application.Interfaces;
using SupportTicket.Domain.Entities;
using SupportTicket.Domain.Enums;
using SupportTicket.Infrastructure.Persistence;
using Xunit;

namespace SupportTicket.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"SupportTicketTests_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<IAppDbContext>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            services.AddScoped<IAppDbContext>(sp =>
                sp.GetRequiredService<AppDbContext>());
        });
    }

    public async Task SeedAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();

        if (!await db.Users.AnyAsync())
        {
            db.Users.AddRange(
                new User { Name = "Alice Agent", Email = "alice.agent@example.com", Role = UserRole.Agent },
                new User { Name = "Bob Supervisor", Email = "bob.supervisor@example.com", Role = UserRole.Supervisor });
            await db.SaveChangesAsync();
        }
    }

    public async Task<Ticket> CreateTicketAsync(TicketStatus status = TicketStatus.Open)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var alice = await db.Users.SingleAsync(u => u.Email == "alice.agent@example.com");
        var bob = await db.Users.SingleAsync(u => u.Email == "bob.supervisor@example.com");

        var now = DateTime.UtcNow;
        var ticket = new Ticket
        {
            Title = $"Test ticket {Guid.NewGuid():N}",
            Description = "Integration test ticket",
            Priority = Priority.Medium,
            Status = status,
            CreatedById = alice.Id,
            AssignedToId = bob.Id,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Tickets.Add(ticket);
        await db.SaveChangesAsync();
        return ticket;
    }
}

public class StatusTransitionTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public StatusTransitionTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData(TicketStatus.Open, TicketStatus.InProgress)]
    [InlineData(TicketStatus.InProgress, TicketStatus.Resolved)]
    [InlineData(TicketStatus.Resolved, TicketStatus.Closed)]
    [InlineData(TicketStatus.Open, TicketStatus.Cancelled)]
    [InlineData(TicketStatus.InProgress, TicketStatus.Cancelled)]
    public async Task ValidTransitions_Succeed(TicketStatus from, TicketStatus to)
    {
        await _factory.SeedAsync();
        var ticket = await _factory.CreateTicketAsync(from);
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync($"/api/tickets/{ticket.Id}/status", new { status = to.ToString() });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);
        body.GetProperty("status").GetString().Should().Be(to.ToString());
    }

    [Theory]
    [InlineData(TicketStatus.Open, TicketStatus.Resolved)]
    [InlineData(TicketStatus.Open, TicketStatus.Closed)]
    [InlineData(TicketStatus.Resolved, TicketStatus.InProgress)]
    [InlineData(TicketStatus.Resolved, TicketStatus.Cancelled)]
    [InlineData(TicketStatus.Closed, TicketStatus.Open)]
    [InlineData(TicketStatus.Closed, TicketStatus.InProgress)]
    [InlineData(TicketStatus.Cancelled, TicketStatus.Open)]
    [InlineData(TicketStatus.Cancelled, TicketStatus.InProgress)]
    public async Task InvalidTransitions_AreRejected(TicketStatus from, TicketStatus to)
    {
        await _factory.SeedAsync();
        var ticket = await _factory.CreateTicketAsync(from);
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync($"/api/tickets/{ticket.Id}/status", new { status = to.ToString() });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Invalid status transition");
        body.Should().Contain(from.ToString());
        body.Should().Contain(to.ToString());
    }
}

public class TicketApiValidationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public TicketApiValidationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateTicket_WithMissingTitle_ReturnsBadRequest()
    {
        await _factory.SeedAsync();
        var client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = await db.Users.Take(2).ToListAsync();

        var response = await client.PostAsJsonAsync("/api/tickets", new
        {
            title = "",
            description = "Valid description",
            priority = "High",
            createdById = users[0].Id,
            assignedToId = users[1].Id
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTicket_WhenMissing_ReturnsNotFound()
    {
        await _factory.SeedAsync();
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/tickets/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
