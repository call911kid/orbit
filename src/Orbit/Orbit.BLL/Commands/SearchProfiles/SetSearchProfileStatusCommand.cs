using FluentValidation;
using Orbit.BLL.Exceptions;
using Orbit.BLL.Interfaces;
using Orbit.DAL.Context;
using Orbit.DAL.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Orbit.BLL.Commands.SearchProfiles;

public sealed record SetSearchProfileStatusCommand(
    Guid Id,
    bool IsActive
) : IRequest;

public class SetSearchProfileStatusCommandHandler : IRequestHandler<SetSearchProfileStatusCommand>
{
    private readonly OrbitDbContext _context;
    private readonly IPostingScheduleManager _scheduleManager;

    public SetSearchProfileStatusCommandHandler(
        OrbitDbContext context,
        IPostingScheduleManager scheduleManager)
    {
        _context = context;
        _scheduleManager = scheduleManager;
    }

    public async Task Handle(SetSearchProfileStatusCommand request, CancellationToken cancellationToken)
    {
        var profile = await _context.SearchProfiles
            .Include(sp => sp.Schedules)
            .FirstOrDefaultAsync(sp => sp.Id == request.Id, cancellationToken);

        if (profile is null)
        {
            throw new EntityNotFoundException(nameof(SearchProfile), request.Id);
        }

        profile.IsActive = request.IsActive;
        await _context.SaveChangesAsync(cancellationToken);

        if (profile.IsActive && !profile.IsDeleted)
        {
            foreach (var schedule in profile.Schedules.Where(s => s.IsEnabled))
            {
                _scheduleManager.ScheduleRecurringIngestion(profile.Id, schedule.Source, schedule.IntervalMinutes);
            }
        }
        else
        {
            foreach (var schedule in profile.Schedules)
            {
                _scheduleManager.RemoveRecurringIngestion(profile.Id, schedule.Source);
            }
        }
    }
}

public class SetSearchProfileStatusCommandValidator : AbstractValidator<SetSearchProfileStatusCommand>
{
    public SetSearchProfileStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
