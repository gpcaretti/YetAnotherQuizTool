namespace Quiz.Application.Sessions {
    public interface IExamSessionAppService {
        Task<int> CountCandidateErrors(UserSessionsRequestDto input);
        Task<int> CountCandidateNotes(UserSessionsRequestDto input);
        Task<int> CountUserSessions(UserSessionsRequestDto input);
        Task<int> CountQuestionNeverAnswered(UserSessionsRequestDto input);
        Task<int> DeleteCandidateNotes(UserSessionsRequestDto input);
        Task<int> DeleteUserSessions(UserSessionsRequestDto input);
        Task<PrepareExamSessionResponseDto> PrepareExamSession(PrepareExamSessionRequestDto input);
        Task<Guid> SaveUserSession(ExamSessionResultsRequestDto input);
        Task<IList<SessionsStatisticsDto>> GetUserStats(UserSessionsRequestDto input);
    }
}