using Microsoft.EntityFrameworkCore;
using Quiz.Domain;
using Quiz.Domain.Exams;

namespace Quiz.Base.Test {

    public abstract class XUnitBaseTest {

        protected QuizDBContext QuizDbContext { get; private set; }
        protected QuizIdentityDBContext QuizIdentityDbContext { get; private set; }

        public XUnitBaseTest() {
            var opts = new DbContextOptionsBuilder<QuizDBContext>()
                .UseInMemoryDatabase(databaseName: "QuizDB_" + RandomString(8))
                .Options;

            QuizDbContext = new QuizDBContext(opts);
            QuizDbContext.Database.EnsureDeleted();
            QuizDbContext.Database.EnsureCreated();

            var optsIdentity = new DbContextOptionsBuilder<QuizIdentityDBContext>()
                .UseInMemoryDatabase(databaseName: "QuizIdentityDB_" + RandomString(8))
                .Options;

            QuizIdentityDbContext = new QuizIdentityDBContext(optsIdentity, new MyOperationalStoreOptions());
            QuizIdentityDbContext.Database.EnsureDeleted();
            QuizIdentityDbContext.Database.EnsureCreated();
        }

        protected async Task<Exam> CreateAndInsertRootExam(Guid? id = null) {
            var entity = CreateRootExam(id);
            QuizDbContext.Exams.Add(entity);
            await QuizDbContext.SaveChangesAsync();
            return entity;
        }

        protected async Task<Exam> CreateAndInsertChildExam(Guid parentId, Guid? childId = null) {
            var entity = CreateChildExam(parentId, childId);
            QuizDbContext.Exams.Add(entity);
            await QuizDbContext.SaveChangesAsync();
            return entity;
        }

        protected async Task<Exam> CreateAndInsertChildExam(Exam parent, Guid? childId = null) {
            var entity = CreateChildExam(parent, childId);
            QuizDbContext.Exams.Add(entity);
            await QuizDbContext.SaveChangesAsync();
            return entity;
        }

        protected Exam CreateRootExam(Guid? id = null) {
            return new Exam(id ?? Guid.NewGuid()) {
                Code = random.Next(1, 99).ToString(),
                CreatedBy = "test",
                CreatedOn = DateTimeOffset.UtcNow,
                Name = "This is a root exam",
                RandomChoicesAllowed = true,
                FullMarks = 100
            };
        }

        protected Exam CreateChildExam(Guid parentId, Guid? childId = null) {
            var parent = QuizDbContext.Exams.First(x => x.Id == parentId);
            return CreateChildExam(parent, childId);
        }

        protected Exam CreateChildExam(Exam parent, Guid? childId = null) {
            return new Exam(childId ?? Guid.NewGuid(), parent.Id) {
                Code = $"{parent.Code}.{RandomDotCode()}",
                CreatedBy = "test",
                CreatedOn = DateTimeOffset.UtcNow,
                Name = "This is the child exam " + random.Next(),
                RandomChoicesAllowed = true,
                FullMarks = 100,
            };
        }

        public string RandomString(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string RandomDotCode() {
            return $"{random.Next(1, 99)}.{random.Next(1, 99)}.{random.Next(1, 99)}";
        }

        private static Random random = new Random();
    }
}