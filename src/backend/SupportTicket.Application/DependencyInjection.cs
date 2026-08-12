using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SupportTicket.Application.Interfaces;
using SupportTicket.Application.Services;
using SupportTicket.Application.Validators;

namespace SupportTicket.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IStatusTransitionService, StatusTransitionService>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IUserService, UserService>();
        services.AddValidatorsFromAssemblyContaining<CreateTicketRequestValidator>();
        return services;
    }
}
