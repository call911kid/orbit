using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Orbit.BLL.Commands.ExcludedCompanies;
using Orbit.BLL.Queries.ExcludedCompanies;
using Orbit.Common.Enums;
using Orbit.PL.Contracts.ExcludedCompanies;
using Orbit.PL.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Orbit.PL.Controllers;

[Authorize]
[ApiController]
[Route("api/excluded-companies")]
public class ExcludedCompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExcludedCompaniesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Agent}")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateExcludedCompanyRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateExcludedCompanyCommand(request.Name);
        var id = await _mediator.Send(command, cancellationToken);

        return Ok(ApiResponse.Success(id));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetExcludedCompaniesQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(ApiResponse.Success(result));
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Agent}")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteExcludedCompanyCommand(id);
        await _mediator.Send(command, cancellationToken);

        return Ok(ApiResponse.Success());
    }
}
