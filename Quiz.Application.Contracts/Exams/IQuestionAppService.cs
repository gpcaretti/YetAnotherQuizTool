using Quiz.Domain.Exams;

namespace Quiz.Application.Exams {
    public interface IQuestionAppService : IQuizApplicationService<Question, QuestionDto, Guid> {
        Task<IList<QuestionAndChoicesDto>> GetQuestionsByExam(ExamQuestionsRequestDto input);
        Task<int> UpdateQuestion(Question entity);
        Task<QuestionAndChoicesDto> FindBySearchKey(string searchKey);
    }
}
