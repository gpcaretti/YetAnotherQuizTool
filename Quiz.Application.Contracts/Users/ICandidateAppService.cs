namespace Quiz.Application.Users {

    public interface ICandidateAppService : IQuizApplicationService<CandidateDto, Guid> {
        Task<string?> GetCandidateName(Guid? candidateId);
        Task<CandidateDto?> GetCurrentUser();
        Task<int> UpdateCandidate(CandidateDto dto);
    }
}
