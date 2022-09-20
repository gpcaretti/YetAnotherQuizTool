using Microsoft.EntityFrameworkCore;
using Quiz.Domain;
using Quiz.Domain.Exams;
using Quiz.Domain.Identity;

namespace Quiz.Base.Test {

    public abstract class MyXUnitBaseTest {

        protected QuizDBContext QuizDbContext { get; private set; }
        protected QuizIdentityDBContext QuizIdentityDbContext { get; private set; }

        public MyXUnitBaseTest() {
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

        protected async Task SeedDatabase() {
            // exams
            var root1 = await CreateAndInsertRootExam();
            await CreateAndInsertChildExam(root1);
            await CreateAndInsertChildExam(root1);
            var child = await CreateAndInsertChildExam(root1);
            await CreateAndInsertChildExam(await CreateAndInsertChildExam(await CreateAndInsertChildExam(root1)));
            await CreateAndInsertChildExam(await CreateAndInsertChildExam(await CreateAndInsertChildExam(root1)));
            var leaf11 = await CreateAndInsertChildExam(await CreateAndInsertChildExam(await CreateAndInsertChildExam(child)));
            var leaf12 = await CreateAndInsertChildExam(await CreateAndInsertChildExam(await CreateAndInsertChildExam(child)));

            var root2 = await CreateAndInsertRootExam();
            await CreateAndInsertChildExam(root2);
            child = await CreateAndInsertChildExam(root2);
            await CreateAndInsertChildExam(await CreateAndInsertChildExam(root2));
            var leaf21 = await CreateAndInsertChildExam(await CreateAndInsertChildExam(await CreateAndInsertChildExam(child)));

            // questions & choices
            await CreateAndInsertQuestion(leaf11);
            await CreateAndInsertQuestion(leaf11);
            await CreateAndInsertQuestion(leaf12);
            await CreateAndInsertQuestion(leaf12);

            await CreateAndInsertQuestion(leaf21);
            await CreateAndInsertQuestion(leaf21);
            await CreateAndInsertQuestion(leaf21);
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
                Code = Random.Shared.Next(1, 99).ToString(),
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
                Name = "This is the child exam " + Random.Shared.Next(),
                RandomChoicesAllowed = true,
                FullMarks = 100,
            };
        }

        protected async Task<Question> CreateAndInsertQuestion(Exam exam, Guid? id = null) {
            var quest = new Question(id ?? Guid.NewGuid(), exam.Id) {
                Code = $"{exam.Code}.1",
                Statement = $"This is the text of question.",
                Position = 0,
                CreatedBy = "test",
                CreatedOn = DateTimeOffset.Now,
                ImageUri = "/a/b/c.png",
            };
            QuizDbContext.Add(quest);
            for (int i = 0; i < 3; i++) {
                QuizDbContext.Add(new Choice(Guid.NewGuid()) {
                    QuestionId = quest.Id,
                    IsCorrect = i == 1,
                    Position = i,
                    Statement = $"this is the choice {i} for the question {quest.Code}",
                    CreatedBy = "test",
                    CreatedOn = DateTimeOffset.Now,
                });
            }
            await QuizDbContext.SaveChangesAsync();

            return await QuizDbContext.Questions
                            .Include(q => q.Exam)
                            .Include(q => q.Choices)
                            .FirstAsync(q => q.Id == quest.Id);
        }

        protected Task<Exam> GetRandomExam() {
            return QuizDbContext.Exams
                    .Include(q => q.Ancestor)
                    .Skip(Random.Shared.Next(0, QuizDbContext.Exams.Count()))
                    .FirstAsync();
        }

        protected Task<Question> GetRandomQuestion() {
            return QuizDbContext.Questions
                    .Include(q => q.Choices)
                    .Skip(Random.Shared.Next(0, QuizDbContext.Questions.Count()))
                    .FirstAsync();
        }

        public string RandomString(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
        }

        public string RandomDotCode() {
            return $"{Random.Shared.Next(1, 99)}.{Random.Shared.Next(1, 99)}.{Random.Shared.Next(1, 99)}";
        }

    }
}