using PatenteN.Quiz.Domain.Exams;

namespace PatenteN.Quiz.Application.Exams {
    public interface IQuestionAppService : IQuizApplicationService<Question, QuestionDto, Guid> {
        Task<QnADto> GetQuestionListByExam(Guid ExamId);
        Task<int> UpdateQuestion(Question entity);
    }
}
