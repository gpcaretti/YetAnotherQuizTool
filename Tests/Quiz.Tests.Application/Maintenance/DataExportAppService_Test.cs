using Quiz.Application.Maintenance;
using Quiz.Domain.Exams;

namespace Quiz.Application.Exams.Maintenance {

    public class DataExportAppService_Test : QuizApplicationTestBase {

        private readonly IDataExportAppService _exportService;

        public DataExportAppService_Test() {
            var examService = new ExamAppService(
                Mock.Of<ILogger<ExamAppService>>(),
                _guidGenerator,
                QuizDbContext,
                _mapper);

            var questionService = new QuestionAppService(
                Mock.Of<ILogger<QuestionAppService>>(),
                _guidGenerator,
                QuizDbContext,
                _mapper);

            _exportService = new ExportAppService(
                Mock.Of<ILogger<ExportAppService>>(),
                examService,
                questionService,
                QuizDbContext,
                _mapper);
        }

        [Fact]
        public async Task ExportToFiles() {
            throw new NotImplementedException();
        }
    }
}