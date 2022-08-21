using Quiz.Domain.Exams;

namespace Quiz.Domain.Test {

    public class QuizContext_Tests : MyXUnitBaseTest {

        public QuizContext_Tests() {
            //SETUP
        }

        [Fact]
        public void IsCreated() {
            QuizDbContext.Exams.Count().ShouldBe(0);
            QuizIdentityDbContext.Users.Count().ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task CreateExam() {
            // prepare
            var nExamsBefore = QuizDbContext.Exams.Count();

            // act
            Exam root = await CreateAndInsertRootExam();
            Exam child = await CreateAndInsertChildExam(root);

            // assert
            QuizDbContext.Exams.Count().ShouldBe(nExamsBefore + 2);
            QuizDbContext.Exams.Count(ex => ex.Id == root.Id).ShouldBe(1);
            QuizDbContext.Exams.Count(ex => ex.Id == child.Id).ShouldBe(1);
        }

    }
}