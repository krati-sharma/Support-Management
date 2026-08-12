using Microsoft.AspNetCore.Mvc;
using SupportTicket.Application.DTOs;
using SupportTicket.Application.Interfaces;
using SupportTicket.Domain.Enums;

namespace SupportTicket.Api.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TicketListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<TicketListItemDto>>> GetTickets(
        [FromQuery] string? search,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        TicketStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            // Reject numeric values and undefined enum members; API contract uses named statuses only.
            if (int.TryParse(status, out _)
                || !Enum.TryParse<TicketStatus>(status, ignoreCase: true, out var parsed)
                || !Enum.IsDefined(parsed))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid status filter",
                    Detail = $"Status '{status}' is not valid. Allowed values: {string.Join(", ", Enum.GetNames<TicketStatus>())}.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            statusFilter = parsed;
        }

        var tickets = await _ticketService.GetTicketsAsync(search, statusFilter, cancellationToken);
        return Ok(tickets);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TicketDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketDetailDto>> GetTicket(int id, CancellationToken cancellationToken)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id, cancellationToken);
        return Ok(ticket);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TicketDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TicketDetailDto>> CreateTicket(
        [FromBody] CreateTicketRequest request,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketService.CreateTicketAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TicketDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketDetailDto>> UpdateTicket(
        int id,
        [FromBody] UpdateTicketRequest request,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketService.UpdateTicketAsync(id, request, cancellationToken);
        return Ok(ticket);
    }

    [HttpPost("{id:int}/status")]
    [ProducesResponseType(typeof(TicketDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketDetailDto>> ChangeStatus(
        int id,
        [FromBody] ChangeStatusRequest request,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketService.ChangeStatusAsync(id, request, cancellationToken);
        return Ok(ticket);
    }

    [HttpGet("{id:int}/valid-transitions")]
    [ProducesResponseType(typeof(ValidTransitionsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ValidTransitionsDto>> GetValidTransitions(int id, CancellationToken cancellationToken)
    {
        var transitions = await _ticketService.GetValidTransitionsAsync(id, cancellationToken);
        return Ok(transitions);
    }

    [HttpPost("{ticketId:int}/comments")]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> AddComment(
        int ticketId,
        [FromBody] CreateCommentRequest request,
        CancellationToken cancellationToken)
    {
        var comment = await _ticketService.AddCommentAsync(ticketId, request, cancellationToken);
        return Created($"/api/tickets/{ticketId}", comment);
    }
}
