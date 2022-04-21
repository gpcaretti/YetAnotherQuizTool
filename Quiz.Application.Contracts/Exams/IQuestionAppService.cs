using Quiz.Application.Exams.Sessions;
using Quiz.Application.Users;
using Quiz.Domain.Exams;

namespace Quiz.Application.Exams {
    public interface IQuestionAppService : IQuizApplicationService<Question, QuestionDto, Guid> {
        Task<ICollection<QuestionAndChoicesDto>> GetRecursiveQuestionsByExam(PrepareExamSessionRequestDto input, BasicCandidateDto candidate = null);
        Task<int> UpdateQuestion(Question entity);
    }
}
