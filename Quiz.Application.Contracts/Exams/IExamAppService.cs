using PatenteN.Quiz.Domain.Exams;

namespace PatenteN.Quiz.Application.Exams {

    public interface IExamAppService : IQuizApplicationService<Exam, ExamDto, Guid> {
        Task<int> UpdateExam(Exam entity);
    }
}
