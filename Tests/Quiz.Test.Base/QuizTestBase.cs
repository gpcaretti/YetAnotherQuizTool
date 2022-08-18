using Quiz.Domain.Exams;
using Quiz.Test.Base;

namespace Quiz.Tests.Base {
    public abstract class QuizTestBase {

        protected readonly DbHelper _dbHelper = new DbHelper();

        protected async Task<Exam> CreateRootExam() {
            var entity = DbHelper.CreateRootExamInstance();
            _dbHelper.QuizDbContext.Add(entity);
            await _dbHelper.QuizDbContext.SaveChangesAsync();
            return entity;
        }

        protected Task<Exam> CreateChildExam(Exam root) {
            return CreateChildExam(root.Id); 
        }

        protected async Task<Exam> CreateChildExam(Guid rootExamId) {
            var entity = DbHelper.CreateChildExamInstance(rootExamId);
            _dbHelper.QuizDbContext.Add(entity);
            await _dbHelper.QuizDbContext.SaveChangesAsync();
            return entity;
        }
    }
}