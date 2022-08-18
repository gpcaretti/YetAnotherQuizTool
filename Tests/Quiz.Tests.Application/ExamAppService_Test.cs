using Microsoft.Extensions.Logging;
using Quiz.Application.Exams;
using Quiz.Domain.Exams;

namespace Quiz.Tests.Application {
    public class ExamAppService_Test : QuizApplicationTestBase {
        private readonly QuestionAppService _questionService;
        private readonly ExamAppService _examService;

        public ExamAppService_Test()  {
            _questionService = new QuestionAppService(
                Mock.Of<ILogger<QuestionAppService>>(),
                _guidGenerator,
                _dbHelper.QuizDbContext,
                _dbHelper.QuizIdentityDbContext,
                _mapper);

            _examService = new ExamAppService(
                Mock.Of<ILogger<ExamAppService>>(),
                _guidGenerator,
                _dbHelper.QuizDbContext,
                _dbHelper.QuizIdentityDbContext,
                _mapper,
                _questionService);
        }

        [Fact]
        public async Task FindById() {
            // prepare
            Exam entity = await CreateRootExam();

            // act
            var dto = await _examService.FindById(entity.Id);

            // assert
            dto.ShouldNotBeNull();
            dto.Id.ShouldBe(entity.Id);

            // act
            dto = await _examService.FindById(Guid.Empty);
            dto.ShouldBeNull();
        }

        [Fact]
        public async Task GetAllRootExams() {
            // prepare
            var nRoots = _dbHelper.QuizDbContext.Exams.Count(ex => !ex.IsDeleted.GetValueOrDefault()  && (ex.Ancestor == null));
            // act
            var dtos = await _examService.GetAllRootExams();
            // assert
            dtos.ShouldNotBeNull();
            dtos.Count().ShouldBe(nRoots);
            dtos.ShouldAllBe(ex => ex.Ancestor == null);

            // prepare
            var root1 = await CreateRootExam();
            var child1 = await CreateChildExam(root1);
            await CreateChildExam(child1);
            await CreateChildExam(root1);
            await CreateChildExam(root1);
            await CreateRootExam();

            // act
            dtos = await _examService.GetAllRootExams();

            // assert
            dtos.ShouldNotBeNull();
            dtos.Count().ShouldBe(nRoots + 2);
            dtos.ShouldAllBe(ex => ex.Ancestor == null);
        }

    }
}
