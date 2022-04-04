using Quiz.Domain.Exams;

namespace Quiz.Application.Exams {
    public interface IQuestionAppService : IQuizApplicationService<Question, QuestionDto, Guid> {
        Task<ICollection<QuestionAndChoicesDto>> GetRecursiveQuestionsByExam(PrepareExamSessionRequestDto input);
        Task<int> UpdateQuestion(Question entity);
        Task<PrepareExamSessionResponseDto> PrepareExamSession(PrepareExamSessionRequestDto input);
    }
}
