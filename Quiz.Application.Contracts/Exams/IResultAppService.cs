using Quiz.Domain.Exams;

namespace Quiz.Application.Exams {
    public interface IResultAppService : IQuizApplicationService<Result, ResultDto, Guid> {
        Task<IEnumerable<QuizAttempt>> GetAttemptHistory(Guid candidateId);
        Task<IEnumerable<QuizReport>> ScoreReport(ReqReport argRpt);
        Task<int> AddResults(List<Result> entity);
        Task<string> GetCertificateString(ReqCertificate argRpt);
    }
}
