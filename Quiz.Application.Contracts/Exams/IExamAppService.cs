using Quiz.Domain.Exams;

namespace Quiz.Application.Exams {

    public interface IExamAppService : IQuizApplicationService<Exam, ExamDto, Guid> {
        Task<int> UpdateExam(Exam entity);
    }
}
