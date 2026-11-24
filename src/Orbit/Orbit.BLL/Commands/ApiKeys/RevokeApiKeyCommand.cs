using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Orbit.BLL.Exceptions;
using Orbit.DAL.Context;
using Orbit.DAL.Models;

namespace Orbit.BLL.Commands.ApiKeys;

public record RevokeApiKeyCommand(Guid Id) : IRequest<bool>;

public class RevokeApiKeyCommandHandler : IRequestHandler<RevokeApiKeyCommand, bool>
{
    private readonly OrbitDbContext _context;

    public RevokeApiKeyCommandHandler(OrbitDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(RevokeApiKeyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ApiKeys
            .FirstOrDefaultAsync(k => k.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new EntityNotFoundException(nameof(ApiKey), request.Id);
        }

        entity.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class RevokeApiKeyCommandValidator : AbstractValidator<RevokeApiKeyCommand>
{
    public RevokeApiKeyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("API Key ID is required.");
    }
}
