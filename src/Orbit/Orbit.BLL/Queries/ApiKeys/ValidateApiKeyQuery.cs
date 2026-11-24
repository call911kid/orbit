using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Orbit.BLL.Interfaces;
using Orbit.Common.Enums;
using Orbit.DAL.Context;

namespace Orbit.BLL.Queries.ApiKeys;

public record ValidateApiKeyQuery(string PlainTextKey) : IRequest<(bool IsValid, Guid KeyId, string Name, Role Role)>;

public class ValidateApiKeyQueryHandler : IRequestHandler<ValidateApiKeyQuery, (bool IsValid, Guid KeyId, string Name, Role Role)>
{
    private readonly OrbitDbContext _context;
    private readonly IApiKeyService _apiKeyService;

    public ValidateApiKeyQueryHandler(OrbitDbContext context, IApiKeyService apiKeyService)
    {
        _context = context;
        _apiKeyService = apiKeyService;
    }

    public async Task<(bool IsValid, Guid KeyId, string Name, Role Role)> Handle(ValidateApiKeyQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.PlainTextKey))
        {
            return (false, Guid.Empty, string.Empty, default);
        }

        var hash = _apiKeyService.HashApiKey(request.PlainTextKey.Trim());
        var now = DateTime.UtcNow;

        var entity = await _context.ApiKeys
            .FirstOrDefaultAsync(k => k.KeyHash == hash, cancellationToken);

        if (entity is null || entity.ExpiresAt <= now)
        {
            return (false, Guid.Empty, string.Empty, default);
        }

        entity.LastUsedAt = now;
        await _context.SaveChangesAsync(cancellationToken);

        return (true, entity.Id, entity.Name, entity.Role);
    }
}
