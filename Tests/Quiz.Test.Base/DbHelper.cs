using Microsoft.EntityFrameworkCore;
using Quiz.Domain;
using Quiz.Domain.Exams;

namespace Quiz.Test.Base {
    public class DbHelper {

        public QuizDBContext QuizDbContext { get; private set; }
        public QuizIdentityDBContext QuizIdentityDbContext { get; private set; }

        public DbHelper() {
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

        public static Exam CreateRootExamInstance() {
            return new Exam(Guid.NewGuid()) {
                Code = random.Next(1, 99).ToString(),
                CreatedBy = "test",
                CreatedOn = DateTimeOffset.UtcNow,
                Name = "This is a root exam",
                RandomChoicesAllowed = true,
                FullMarks = 100
            };
        }

        public static Exam CreateChildExamInstance(Guid parent) {
            return new Exam(Guid.NewGuid(), parent) {
                Code = RandomDotCode(),
                CreatedBy = "test",
                CreatedOn = DateTimeOffset.UtcNow,
                Name = "This is the child exam " + random.Next(),
                RandomChoicesAllowed = true,
                FullMarks = 100,
            };
        }


        public static string RandomString(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomDotCode() {
            return $"{random.Next(1, 99)}.{random.Next(1, 99)}.{random.Next(1, 99)}.{random.Next(1, 99)}";
        }

        private static Random random = new Random();

    }
}