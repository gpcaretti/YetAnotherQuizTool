using System.Diagnostics.CodeAnalysis;
using Quiz.Application.Exams.Sessions;
using Quiz.Application.Users;
using Quiz.Domain.Exams;

namespace Quiz.Application.Exams {

    public interface IExamAppService : IQuizApplicationService<Exam, ExamDto, Guid> {
        Task<PrepareExamSessionResponseDto> PrepareExamSession(PrepareExamSessionRequestDto input, BasicCandidateDto candidate = null);
        Task<int> UpdateExam(Exam entity);
        Task<Guid> SaveUserSession(ExamSessionResultsRequestDto input, [NotNull]BasicCandidateDto candidate);
    }
}
