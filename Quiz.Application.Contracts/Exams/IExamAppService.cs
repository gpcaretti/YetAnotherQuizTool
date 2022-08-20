namespace Quiz.Application.Exams {

    public interface IExamAppService : IQuizApplicationService<ExamDto, Guid> {
        Task<IList<ExamDto>> GetAllRootExams();
        Task<IList<Guid>> GetRecursiveExamIds(RecursiveExamsRequestDto input);
        Task<int> UpdateExam(UpdateExamDto input);
        //TODO: Task<int> UpdateExam(Exam entity);
    }
}
