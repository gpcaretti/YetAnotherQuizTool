using Quiz.Application.Exams;
using Quiz.Application.Users;

namespace Quiz.Application.Sessions.Tests {

    public class ExamSessionAppService_Tests : QuizApplicationTestBase {

        private readonly IExamSessionAppService _service;

        public ExamSessionAppService_Tests() {
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

            var candidateAppService = new NullCandidateAppService(
                Mock.Of<ILogger<NullCandidateAppService>>(),
                _guidGenerator,
                QuizDbContext,
                _mapper);

            _service = new ExamSessionAppService(
                Mock.Of<ILogger<ExamSessionAppService>>(),
                _guidGenerator,
                QuizDbContext,
                _mapper,
                candidateAppService,
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

        [Fact()]
        public void GetUserSessions() {
            throw new NotImplementedException();
        }

        [Fact()]
        public void GetUserSessionsWithAnswers() {
            throw new NotImplementedException();
        }

        [Fact()]
        public void GetUserStats() {
            throw new NotImplementedException();
        }

        [Fact()]
        public void SaveUserSession() {
            throw new NotImplementedException();
        }

        [Fact()]
        public void DeleteUserSessions() {
            throw new NotImplementedException();
        }

        [Fact()]
        public void DeleteCandidateNotes() {
            throw new NotImplementedException();
        }

        [Fact()]
        public void CountUserSessions() {
            throw new NotImplementedException();
        }

        [Fact()]
        public void CountCandidateNotes() {
            throw new NotImplementedException();
        }

        [Fact()]
        public void CountCandidateErrors() {
            throw new NotImplementedException();
        }

        [Fact()]
        public void CountQuestionNeverAnswered() {
            throw new NotImplementedException();
        }
    }
}