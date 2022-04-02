using PatenteN.Quiz.Domain.Users;

namespace PatenteN.Quiz.Application.Users {

    public interface ICandidateAppService : IQuizApplicationService<Candidate, CandidateDto, Guid> {
        Task<int> UpdateCandidate(CandidateDto dto);
    }
}
