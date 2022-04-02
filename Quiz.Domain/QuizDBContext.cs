using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PatenteN.Quiz.Domain.Exams;
using PatenteN.Quiz.Domain.Users;

namespace PatenteN.Quiz.Domain {
    public class QuizDBContext : DbContext {
        public QuizDBContext() {
        }

        public QuizDBContext(DbContextOptions<QuizDBContext> options) : base(options) {
        }

        public virtual DbSet<Candidate> Candidates { get; set; }
        public virtual DbSet<Exam> Exams { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Choice> Choices { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Result> QuizResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // tables prefix
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes()) {
                entity.SetTableName("qz" + entity.GetTableName());
            }

            //// only 1 choice per question is correct
            //modelBuilder.Entity<Choice>()
            //    .HasIndex(u => new { u.QuestionId, u.IsCorrect })
            //    .IsUnique();

            modelBuilder.Entity<QuizAttempt>(eb => {
                eb.HasNoKey();
                eb.ToView(null);
            });

            modelBuilder.Entity<QuizReport>(eb => {
                eb.HasNoKey();
                eb.ToView(null);
            });
        }
    }
}
