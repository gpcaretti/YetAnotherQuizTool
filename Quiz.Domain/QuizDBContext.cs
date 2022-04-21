using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Quiz.Domain.Exams;
using Quiz.Domain.Exams.Sessions;
using Quiz.Domain.Users;

namespace Quiz.Domain {
    public class QuizDBContext : DbContext {
        public QuizDBContext() {
        }

        public QuizDBContext(DbContextOptions<QuizDBContext> options) : base(options) {
        }

        public virtual DbSet<Candidate> Candidates { get; set; }
        public virtual DbSet<Exam> Exams { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Choice> Choices { get; set; }
        public virtual DbSet<ExamSession> ExamSessions { get; set; }
        public virtual DbSet<ExamSessionItem> ExamSessionItems { get; set; }
        public virtual DbSet<CandidateNote> CandidateNotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // tables prefix
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes()) {
                entity.SetTableName("qz" + entity.GetTableName());
            }

            modelBuilder.Entity<CandidateNote>()
                .HasKey(u => new { u.CandidateId, u.QuestionId })
                ;

            //// only 1 choice per question is correct
            //modelBuilder.Entity<Choice>()
            //    .HasIndex(u => new { u.QuestionId, u.IsCorrect })
            //    .IsUnique();

            modelBuilder.Entity<Question>()
                .HasIndex(u => u.Position)
                //.IsUnique()
                ;

            modelBuilder.Entity<ExamSession>()
                .HasIndex(u => new { u.CandidateId, u.ExamId })
                //.IsUnique()
                ;

            modelBuilder.Entity<CandidateNote>()
                .HasIndex(u => new { u.ExamId })
                //.IsUnique()
                ;

            //modelBuilder.Entity<QuizAttempt>(eb => {
            //    eb.HasNoKey();
            //    eb.ToView(null);
            //});
        }
    }
}
