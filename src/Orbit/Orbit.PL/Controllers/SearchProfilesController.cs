using AutoMapper;
using Orbit.BLL.Commands.SearchProfiles;
using Orbit.BLL.Queries.SearchProfiles;
using Orbit.Common.Enums;
using Orbit.PL.Contracts.SearchProfiles;
using Orbit.PL.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Orbit.PL.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SearchProfilesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public SearchProfilesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Agent}")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSearchProfileRequest request)
    {
        var command = new CreateSearchProfileCommand(
            request.Name,
            request.Description,
            request.TargetJobTitles,
            request.TargetLocations,
            request.MaxPostingAgeMinutes,
            request.Workplace,
            request.Experience,
            request.JobType,
            request.TelegramChatId,
            request.BotToken
        );

        var id = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { id }, ApiResponse.Success(id));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllSearchProfilesQuery();
        var responses = await _mediator.Send(query);
        var result = _mapper.Map<List<SearchProfileResponse>>(responses);

        if (User.IsInRole(Roles.Guest))
        {
            result = result.Select(RedactSecrets).ToList();
        }

        return Ok(ApiResponse.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetSearchProfileByIdQuery(id);
        var response = await _mediator.Send(query);
        var result = _mapper.Map<SearchProfileResponse>(response);

        if (User.IsInRole(Roles.Guest))
        {
            result = RedactSecrets(result);
        }

        return Ok(ApiResponse.Success(result));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSearchProfileRequest request)
    {
        // Guest callers cannot read or overwrite the Telegram bot token; empty string preserves the stored token.
        var botToken = User.IsInRole(Roles.Guest) ? string.Empty : request.BotToken;

        var command = new UpdateSearchProfileCommand(
            id,
            request.Name,
            request.Description,
            request.TargetJobTitles,
            request.TargetLocations,
            request.MaxPostingAgeMinutes,
            request.Workplace,
            request.Experience,
            request.JobType,
            request.TelegramChatId,
            botToken
        );

        await _mediator.Send(command);

        return Ok(ApiResponse.Success());
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Agent}")]
    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetSearchProfileStatusRequest request)
    {
        var command = new SetSearchProfileStatusCommand(id, request.IsActive);
        await _mediator.Send(command);

        return Ok(ApiResponse.Success());
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Agent}")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteSearchProfileCommand(id);
        await _mediator.Send(command);

        return Ok(ApiResponse.Success());
    }

    private static SearchProfileResponse RedactSecrets(SearchProfileResponse response) =>
        response with { BotToken = string.Empty };
}
