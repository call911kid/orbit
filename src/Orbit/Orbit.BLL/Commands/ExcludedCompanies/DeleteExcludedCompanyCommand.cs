using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Orbit.BLL.Exceptions;
using Orbit.DAL.Context;
using Orbit.DAL.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Orbit.BLL.Commands.ExcludedCompanies;

public record DeleteExcludedCompanyCommand(Guid Id) : IRequest<bool>;

public class DeleteExcludedCompanyCommandHandler : IRequestHandler<DeleteExcludedCompanyCommand, bool>
{
    private readonly OrbitDbContext _context;

    public DeleteExcludedCompanyCommandHandler(OrbitDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteExcludedCompanyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ExcludedCompanies
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new EntityNotFoundException(nameof(ExcludedCompany), request.Id);
        }

        entity.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public class DeleteExcludedCompanyCommandValidator : AbstractValidator<DeleteExcludedCompanyCommand>
{
    public DeleteExcludedCompanyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
