using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Orbit.Common.Enums;
using Orbit.DAL.Context;

namespace Orbit.BLL.Queries.ApiKeys;

public record GetAllApiKeysQuery : IRequest<List<GetAllApiKeysResponse>>;

public sealed record GetAllApiKeysResponse(
    Guid Id,
    string Name,
    string KeyPrefix,
    Role Role,
    DateTime ExpiresAt,
    DateTime? LastUsedAt,
    DateTime CreatedAt
);

public class GetAllApiKeysQueryHandler : IRequestHandler<GetAllApiKeysQuery, List<GetAllApiKeysResponse>>
{
    private readonly OrbitDbContext _context;
    private readonly IMapper _mapper;

    public GetAllApiKeysQueryHandler(OrbitDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<GetAllApiKeysResponse>> Handle(GetAllApiKeysQuery request, CancellationToken cancellationToken)
    {
        var entities = await _context.ApiKeys
            .AsNoTracking()
            .OrderByDescending(k => k.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<GetAllApiKeysResponse>>(entities);
    }
}
