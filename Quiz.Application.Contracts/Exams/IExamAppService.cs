using Quiz.Domain.Exams;

namespace Quiz.Application.Exams {

    public interface IExamAppService : IQuizApplicationService<Exam, ExamDto, Guid> {
        Task<IList<ExamDto>> GetAllRootExams();
        Task<IList<Guid>> GetRecursiveExamIds(RecursiveExamsRequestDto input);
        //TODO: Task<int> UpdateExam(Exam entity);
    }
}
