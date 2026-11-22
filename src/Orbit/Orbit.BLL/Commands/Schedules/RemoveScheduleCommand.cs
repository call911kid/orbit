using FluentValidation;
using Orbit.BLL.Exceptions;
using Orbit.BLL.Interfaces;
using Orbit.Common.Enums;
using Orbit.DAL.Context;
using Orbit.DAL.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Orbit.BLL.Commands.Schedules;

public sealed record RemoveScheduleCommand(
    Guid SearchProfileId,
    PostingSource Source
) : IRequest;

public class RemoveScheduleCommandHandler : IRequestHandler<RemoveScheduleCommand>
{
    private readonly OrbitDbContext _context;
    private readonly IPostingScheduleManager _scheduleManager;

    public RemoveScheduleCommandHandler(
        OrbitDbContext context,
        IPostingScheduleManager scheduleManager)
    {
        _context = context;
        _scheduleManager = scheduleManager;
    }

    public async Task Handle(RemoveScheduleCommand request, CancellationToken cancellationToken)
    {
        var profile = await _context.SearchProfiles
            .Include(sp => sp.Schedules)
            .FirstOrDefaultAsync(sp => sp.Id == request.SearchProfileId, cancellationToken);

        if (profile is null)
        {
            throw new EntityNotFoundException(nameof(SearchProfile), request.SearchProfileId);
        }

        var schedule = profile.Schedules.FirstOrDefault(s => s.Source == request.Source);
        if (schedule is not null)
        {
            profile.Schedules.Remove(schedule);
            await _context.SaveChangesAsync(cancellationToken);
            _scheduleManager.RemoveRecurringIngestion(profile.Id, request.Source);
        }
    }
}

public class RemoveScheduleCommandValidator : AbstractValidator<RemoveScheduleCommand>
{
    public RemoveScheduleCommandValidator()
    {
        RuleFor(x => x.SearchProfileId)
            .NotEmpty().WithMessage("SearchProfileId is required.");

        RuleFor(x => x.Source)
            .IsInEnum().WithMessage("Invalid posting source value.");
    }
}
