using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Orbit.BLL.Interfaces;
using Orbit.Common.Enums;
using Orbit.DAL.Context;
using Orbit.DAL.Models;

namespace Orbit.BLL.Commands.ApiKeys;

public sealed record CreateApiKeyResponse(
    Guid Id,
    string Name,
    string RawKey,
    string KeyPrefix,
    Role Role,
    DateTime ExpiresAt,
    DateTime CreatedAt);

public record CreateApiKeyCommand(
    string Name,
    Role Role,
    int ValidForMinutes) : IRequest<CreateApiKeyResponse>;

public class CreateApiKeyCommandHandler : IRequestHandler<CreateApiKeyCommand, CreateApiKeyResponse>
{
    private readonly OrbitDbContext _context;
    private readonly IApiKeyService _apiKeyService;

    public CreateApiKeyCommandHandler(OrbitDbContext context, IApiKeyService apiKeyService)
    {
        _context = context;
        _apiKeyService = apiKeyService;
    }

    public async Task<CreateApiKeyResponse> Handle(CreateApiKeyCommand request, CancellationToken cancellationToken)
    {
        var (rawKey, keyPrefix, keyHash) = _apiKeyService.GenerateApiKey();
        var expiresAt = DateTime.UtcNow.AddMinutes(request.ValidForMinutes);

        var entity = new ApiKey
        {
            Name = request.Name.Trim(),
            KeyPrefix = keyPrefix,
            KeyHash = keyHash,
            Role = request.Role,
            ExpiresAt = expiresAt
        };

        _context.ApiKeys.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateApiKeyResponse(
            entity.Id,
            entity.Name,
            rawKey,
            entity.KeyPrefix,
            entity.Role,
            entity.ExpiresAt,
            entity.CreatedAt);
    }
}

public class CreateApiKeyCommandValidator : AbstractValidator<CreateApiKeyCommand>
{
    public CreateApiKeyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("A valid role is required.")
            .Must(r => r is Role.Guest or Role.Agent)
            .WithMessage("Database API key role must be either 'Guest' or 'Agent'.");

        RuleFor(x => x.ValidForMinutes)
            .GreaterThan(0)
            .WithMessage("Validity period in minutes is required and must be greater than 0.");
    }
}
