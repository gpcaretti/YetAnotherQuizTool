
using Quiz.Domain.Exams;

namespace Quiz.Tests.Domain {

    public class QuizContext_Tests {

        private readonly DbHelper _dbHelper = new DbHelper();

        public QuizContext_Tests() {
            //SETUP
        }

        [Fact]
        public void IsCreated() {
            _dbHelper.QuizDbContext.Exams.Count().ShouldBe(0);
            _dbHelper.QuizIdentityDbContext.Users.Count().ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task CreateExam() {
            // prepare
            var nExamsBefore = _dbHelper.QuizDbContext.Exams.Count();

            // act
            Exam exam = DbHelper.CreateRootExamInstance();
            _dbHelper.QuizDbContext.Exams.Add(exam);
            await _dbHelper.QuizDbContext.SaveChangesAsync();

            // assert
            _dbHelper.QuizDbContext.Exams.Count().ShouldBe(nExamsBefore + 1);
            _dbHelper.QuizDbContext.Exams.Count(ex => ex.Id == exam.Id).ShouldBe(1);
        }

    }
}