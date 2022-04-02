using PatenteN.Quiz.Domain.Exams;

namespace PatenteN.Quiz.Application.Exams {

    public interface IExamAppService : IQuizApplicationService<Exam, ExamDto, Guid> {
        Task<ICollection<QuestionDto>> GetQuestionsByExamId(Guid examId);
        Task<int> UpdateExam(Exam entity);
    }
}
