using Microsoft.AspNetCore.Identity;
using Quiz.Application.Users;
using Quiz.Domain.Identity;

namespace Quiz.Blazor.ServerApp {

    /// <summary>
    ///     Automatically invoked by IServiceCollection.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
    /// </summary>
    public class AutomapperProfile : AutoMapper.Profile /*Quiz.Application.AutomapperProfile */{

        public AutomapperProfile() {
            CandidateMaps();
        }

        private void CandidateMaps() {
            CreateMap<IdentityUser<Guid>, CandidateDto>()
                ;

            CreateMap<ApplicationUser, CandidateDto>()
                ;

            CreateMap<CandidateDto, IdentityUser<Guid>>()
                //.ForMember(dst => dst.CreatedOn, src => src.Ignore())
                ;
        }

    }
}
