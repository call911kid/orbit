using AutoMapper;
using Orbit.BLL.Commands.JobPostings;
using Orbit.DAL.Models;

namespace Orbit.BLL.Mapping;

public class JobPostingMappingProfile : Profile
{
    public JobPostingMappingProfile()
    {
        CreateMap<CreateJobPostingItem, JobPosting>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title.Trim()))
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company.Trim()))
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url.Trim()))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Location) ? string.Empty : src.Location.Trim()))
            .ForMember(dest => dest.DescriptionSnippet, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.DescriptionSnippet) ? null : src.DescriptionSnippet.Trim()))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}
