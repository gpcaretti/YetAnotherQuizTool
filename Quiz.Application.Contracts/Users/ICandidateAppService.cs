using Microsoft.AspNetCore.Identity;

namespace Quiz.Application.Users {

    public interface ICandidateAppService : IQuizApplicationBaseService<IdentityUser<Guid>, CandidateDto, Guid> {
        Task<int> UpdateCandidate(CandidateDto dto);
    }
}
