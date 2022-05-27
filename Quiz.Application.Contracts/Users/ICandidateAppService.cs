namespace Quiz.Application.Users {

    public interface ICandidateAppService : IQuizApplicationService<CandidateDto, Guid> {
        Task<int> UpdateCandidate(CandidateDto dto);
    }
}
