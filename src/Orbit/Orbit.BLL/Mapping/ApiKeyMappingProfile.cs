using AutoMapper;
using Orbit.BLL.Queries.ApiKeys;
using Orbit.DAL.Models;

namespace Orbit.BLL.Mapping;

public class ApiKeyMappingProfile : Profile
{
    public ApiKeyMappingProfile()
    {
        CreateMap<ApiKey, GetAllApiKeysResponse>();
    }
}
