namespace Quiz.Application.Exams {
    public interface IQuestionAppService : IQuizApplicationService<QuestionDto, Guid> {
        Task<IList<QuestionAndChoicesDto>> GetQuestionsByIds(params Guid[] input);
        Task<IList<QuestionAndChoicesDto>> GetQuestionsByExam(ExamQuestionsRequestDto input);
        //Task<int> UpdateQuestion(Question entity);
        Task<QuestionAndChoicesDto> FindBySearchKey(string searchKey);
    }
}
