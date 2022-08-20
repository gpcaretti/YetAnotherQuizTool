namespace Quiz.Application.Exams.Tests {
    public class QuestionAppService_Tests : QuizApplicationTestBase {

        private readonly IQuestionAppService _questionService;

        public QuestionAppService_Tests() {
            _questionService = new QuestionAppService(
                Mock.Of<ILogger<QuestionAppService>>(),
                _guidGenerator,
                QuizDbContext,
                QuizIdentityDbContext,
                _mapper);
        }

        [Fact()]
        public void FindBySearchKey() {
            throw new NotImplementedException();
        }

        [Fact()]
        public void GetQuestionsByIds() {
            throw new NotImplementedException();
        }

        [Fact()]
        public void GetQuestionsByExam() {
            throw new NotImplementedException();
        }

        [Fact()]
        public void CreateQuestion() {
            throw new NotImplementedException();
        }

        [Fact()]
        public void UpdateQuestion() {
            throw new NotImplementedException();
        }
    }
}