using AutoMapper;
using IssueLog.API.Dtos;
using IssueLog.API.Models;

namespace IssueLog.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<IssueForAddDto, Issue>();
            
        }
    }
}