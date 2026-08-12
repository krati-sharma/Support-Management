using FluentValidation;
using SupportTicket.Application.DTOs;

namespace SupportTicket.Application.Validators;

public class CreateTicketRequestValidator : AbstractValidator<CreateTicketRequest>
{
    public CreateTicketRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(4000);

        RuleFor(x => x.Priority)
            .NotNull().WithMessage("Priority is required.")
            .IsInEnum().WithMessage("Priority must be Low, Medium, or High.");

        RuleFor(x => x.CreatedById)
            .GreaterThan(0).WithMessage("CreatedById is required.");

        RuleFor(x => x.AssignedToId)
            .GreaterThan(0).WithMessage("AssignedToId is required.");
    }
}

public class UpdateTicketRequestValidator : AbstractValidator<UpdateTicketRequest>
{
    public UpdateTicketRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(4000);

        RuleFor(x => x.Priority)
            .NotNull().WithMessage("Priority is required.")
            .IsInEnum().WithMessage("Priority must be Low, Medium, or High.");

        RuleFor(x => x.AssignedToId)
            .GreaterThan(0).WithMessage("AssignedToId is required.");
    }
}

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MaximumLength(2000);

        RuleFor(x => x.CreatedById)
            .GreaterThan(0).WithMessage("CreatedById is required.");
    }
}

public class ChangeStatusRequestValidator : AbstractValidator<ChangeStatusRequest>
{
    public ChangeStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotNull().WithMessage("Status is required.")
            .IsInEnum().WithMessage("Status must be a valid ticket status.");
    }
}
