using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Quiz.Domain.Exams;
using Quiz.Domain.Exams.Sessions;

namespace Quiz.Domain {

    public class QuizDBContext
        //: IdentityDbContext<ApplicationUser, ApplicationRole, string> {
        //: ApiAuthorizationDbContext<ApplicationUser> {
        : DbContext {

        public QuizDBContext() {
        }

        public QuizDBContext(
            DbContextOptions<QuizDBContext> options)
            : base(options) {
        }

        public virtual DbSet<Exam> Exams { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Choice> Choices { get; set; }
        public virtual DbSet<ExamSession> ExamSessions { get; set; }
        public virtual DbSet<ExamSessionItem> ExamSessionItems { get; set; }
        public virtual DbSet<CandidateNote> CandidateNotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            // tables prefix
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes()) {
                if (entity.GetTableName().StartsWith("AspNet", StringComparison.InvariantCultureIgnoreCase)) continue;
                entity.SetTableName("qz" + entity.GetTableName());
            }

            DoCreateFKsAndIndexes(modelBuilder);
            DoSeedData(modelBuilder);
        }

        private void DoCreateFKsAndIndexes(ModelBuilder modelBuilder) {
            // index Code for Question
            modelBuilder.Entity<Exam>()
                .HasIndex(u => u.Code)
                //.IsUnique()
                ;

            // index Position for Question
            modelBuilder.Entity<Question>()
                .HasIndex(u => u.Position)
                //.IsUnique()
                ;

            // index CandidateId for CandidateNote
            modelBuilder.Entity<CandidateNote>()
                .HasKey(u => new { u.CandidateId, u.QuestionId })
                ;
            // index ExamId for CandidateNote
            modelBuilder.Entity<CandidateNote>()
                .HasIndex(u => new { u.ExamId })
                //.IsUnique()
                ;

            // index  CandidateId + ExamId for ExamSession
            modelBuilder.Entity<ExamSession>()
                .HasIndex(u => new { u.CandidateId, u.ExamId })
                //.IsUnique()
                ;

            // index  SessionId for ExamSessionItems
            modelBuilder.Entity<ExamSessionItem>()
                .HasIndex(u => new { u.SessionId })
                //.IsUnique()
                ;

            //// only 1 choice per question is correct
            //modelBuilder.Entity<Choice>()
            //    .HasIndex(u => new { u.QuestionId, u.IsCorrect })
            //    .IsUnique();

            //modelBuilder.Entity<QuizAttempt>(eb => {
            //    eb.HasNoKey();
            //    eb.ToView(null);
            //});
        }

        private void DoSeedData(ModelBuilder modelBuilder) {
        }

    }
}
