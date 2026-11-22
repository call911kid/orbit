using AutoMapper;
using Orbit.DAL.Models;
using Orbit.Providers.Contracts;

namespace Orbit.BLL.Mapping;

public class JobPostingMappingProfile : Profile
{
    public JobPostingMappingProfile()
    {
        CreateMap<ExternalPosting, JobPosting>()
            .ForMember(dest => dest.ExternalJobId, opt => opt.MapFrom(src => src.ExternalId))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location ?? string.Empty))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}
