using System.Collections.Generic;
using System.Threading.Tasks;
using PatenteN.Quiz.Domain.Exams;

namespace PatenteN.Quiz.Application.Exams {
    public interface IResultAppService : IQuizApplicationService<Result, ResultDto> {
        Task<IEnumerable<QuizAttempt>> GetAttemptHistory(string argCandidateID);
        Task<IEnumerable<QuizReport>> ScoreReport(ReqReport argRpt);
        Task<int> AddResults(List<Result> entity);
        Task<string> GetCertificateString(ReqCertificate argRpt);
    }
}
