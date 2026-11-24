using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orbit.BLL.Commands.ApiKeys;
using Orbit.BLL.Queries.ApiKeys;
using Orbit.Common.Enums;
using Orbit.PL.Contracts.ApiKeys;
using Orbit.PL.Responses;

namespace Orbit.PL.Controllers;

[ApiController]
[Route("api/api-keys")]
[Authorize(Roles = Roles.Admin)]
public class ApiKeysController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ApiKeysController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllApiKeysQuery();
        var responses = await _mediator.Send(query, cancellationToken);
        var result = _mapper.Map<List<ApiKeyResponse>>(responses);

        return Ok(ApiResponse.Success(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApiKeyRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateApiKeyCommand(request.Name, request.Role, request.ValidForMinutes);
        var response = await _mediator.Send(command, cancellationToken);
        var result = _mapper.Map<CreatedApiKeyResponse>(response);

        return Ok(ApiResponse.Success(result));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Revoke(Guid id, CancellationToken cancellationToken)
    {
        var command = new RevokeApiKeyCommand(id);
        await _mediator.Send(command, cancellationToken);

        return Ok(ApiResponse.Success("API key revoked successfully."));
    }
}
