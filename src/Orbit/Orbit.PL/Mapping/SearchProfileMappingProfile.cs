using AutoMapper;
using Orbit.BLL.Queries.SearchProfiles;
using Orbit.PL.Contracts.SearchProfiles;

namespace Orbit.PL.Mapping;

public class SearchProfileMappingProfile : Profile
{
    public SearchProfileMappingProfile()
    {
        CreateMap<GetAllSearchProfilesScheduleResponse, ScheduleResponse>();
        CreateMap<GetAllSearchProfilesResponse, SearchProfileResponse>();

        CreateMap<GetSearchProfileByIdScheduleResponse, ScheduleResponse>();
        CreateMap<GetSearchProfileByIdResponse, SearchProfileResponse>();
    }
}
