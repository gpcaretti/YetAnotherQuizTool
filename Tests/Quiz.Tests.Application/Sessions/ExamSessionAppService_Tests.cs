using Xunit;
using Quiz.Application.Sessions;
using Quiz.Application.Exams;

namespace Quiz.Application.Sessions.Tests {

    public class ExamSessionAppService_Tests : QuizApplicationTestBase {

        private readonly IExamSessionAppService _service;

        public ExamSessionAppService_Tests() {
            var examService = new ExamAppService(
                Mock.Of<ILogger<ExamAppService>>(),
                _guidGenerator,
                QuizDbContext,
                QuizIdentityDbContext,
                _mapper);

            var questionService = new QuestionAppService(
                Mock.Of<ILogger<QuestionAppService>>(),
                _guidGenerator,
                QuizDbContext,
                QuizIdentityDbContext,
                _mapper);

            _service = new ExamSessionAppService(
                Mock.Of<ILogger<ExamSessionAppService>>(),
                _guidGenerator,
                QuizDbContext,
                QuizIdentityDbContext,
                _mapper,
                examService,
                questionService);
        }

        [Fact()]
        public void PrepareExamSession() {
            throw new NotImplementedException();
        }

        [Fact()]
        public void PrepareExamSession1() {
            throw new NotImplementedException();
        }
    }
}