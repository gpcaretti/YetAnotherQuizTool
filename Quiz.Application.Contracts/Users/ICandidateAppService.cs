using Quiz.Domain.Users;

namespace Quiz.Application.Users {

    public interface ICandidateAppService : IQuizApplicationService<Candidate, CandidateDto, Guid> {
        Task<int> UpdateCandidate(CandidateDto dto);
    }
}
