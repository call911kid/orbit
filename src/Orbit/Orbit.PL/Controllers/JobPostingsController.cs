using System;
using System.Threading;
using System.Threading.Tasks;
using Orbit.BLL.Queries.JobPostings;
using Orbit.Common.Enums;
using Orbit.Common.Pagination;
using Orbit.PL.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Orbit.PL.Controllers;

[Authorize]
[ApiController]
[Route("api/job-postings")]
public class JobPostingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobPostingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PagingQuery paging,
        [FromQuery] PostingSource? source,
        [FromQuery] string? search,
        [FromQuery] bool? isBlocked,
        CancellationToken cancellationToken)
    {
        var query = new GetJobPostingsQuery(paging, source, search, isBlocked);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(ApiResponse.Success(result));
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var stats = await _mediator.Send(new GetJobPostingStatsQuery(), cancellationToken);
        return Ok(ApiResponse.Success(stats));
    }
}
