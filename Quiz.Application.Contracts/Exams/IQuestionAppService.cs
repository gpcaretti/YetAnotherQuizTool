using PatenteN.Quiz.Domain.Exams;

namespace PatenteN.Quiz.Application.Exams {
    public interface IQuestionAppService : IQuizApplicationService<Question, QuestionDto, Guid> {
        Task<ICollection<QuestionAndChoicesDto>> GetRecursiveQuestionsByExam(QuestionsByExamRequestDto input);
        Task<QnADto> GetQuestionListByExam(Guid ExamId);
        Task<int> UpdateQuestion(Question entity);
        Task<QnADto> PrepareExamAttempt(QuestionsByExamRequestDto input);
    }
}
