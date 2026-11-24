using AutoMapper;
using Orbit.BLL.Commands.ApiKeys;
using Orbit.BLL.Queries.ApiKeys;
using Orbit.PL.Contracts.ApiKeys;

namespace Orbit.PL.Mapping;

public class ApiKeyMappingProfile : Profile
{
    public ApiKeyMappingProfile()
    {
        CreateMap<GetAllApiKeysResponse, ApiKeyResponse>();
        CreateMap<CreateApiKeyResponse, CreatedApiKeyResponse>();
    }
}
