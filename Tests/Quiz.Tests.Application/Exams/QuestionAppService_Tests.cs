namespace Quiz.Application.Exams.Tests {
    public class QuestionAppService_Tests : QuizApplicationTestBase {

        private readonly IQuestionAppService _questionService;

        public QuestionAppService_Tests() {
            _questionService = new QuestionAppService(
                Mock.Of<ILogger<QuestionAppService>>(),
                _guidGenerator,
                QuizDbContext,
                _mapper);
        }

        [Fact()]
        public async Task FindById() {
            // prepare
            await SeedDatabase();
            var entity = await GetRandomQuestion();

            // act
            var output = await _questionService.FindById(entity.Id);

            // assert
            output.ShouldNotBeNull();
            output.Id.ShouldBe(entity.Id);
            output.Statement.ShouldBe(entity.Statement);
            output.Code.ShouldBe(entity.Code);
            output.ExamId.ShouldBe(entity.ExamId);
            output.ImageUri.ShouldBe(entity.ImageUri);
            output.Position.ShouldBe(entity.Position ?? -1);

            // act and assert
            output = await _questionService.FindById(Guid.Empty);
            output.ShouldBeNull();
        }

        [Fact()]
        public async Task FindBySearchKey() {
            throw new NotImplementedException();
        }

        [Fact()]
        public async Task GetQuestionsByIds() {
            throw new NotImplementedException();
        }

        [Fact()]
        public async Task GetQuestionsByExam() {
            throw new NotImplementedException();
        }

        [Fact()]
        public async Task CreateQuestion() {
            throw new NotImplementedException();
        }

        [Fact()]
        public async Task UpdateQuestion() {
            throw new NotImplementedException();
        }
    }
}