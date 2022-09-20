using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Quiz.Domain.Exams;
using Quiz.Domain.Exams.Sessions;

namespace Quiz.Domain {

    public class QuizDBContext
        //: IdentityDbContext<ApplicationUser, ApplicationRole, string> {
        //: ApiAuthorizationDbContext<ApplicationUser> {
        : DbContext {

        private readonly IConfiguration _configuration;

        //public QuizDBContext() {
        //}

        protected QuizDBContext(DbContextOptions options)
            : base(options) {
        }

        protected QuizDBContext(
            DbContextOptions options, IConfiguration configuration)
            : this(options) {
            _configuration = configuration;
        }

        public QuizDBContext(DbContextOptions<QuizDBContext> options)
            : base(options) {
        }

        public QuizDBContext(
            DbContextOptions<QuizDBContext> options, IConfiguration configuration)
            : this(options) {
            _configuration = configuration;
        }

        public virtual DbSet<Exam> Exams { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Choice> Choices { get; set; }
        public virtual DbSet<ExamSession> ExamSessions { get; set; }
        public virtual DbSet<ExamSessionItem> ExamSessionItems { get; set; }
        public virtual DbSet<CandidateNote> CandidateNotes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
            //var connString = _configuration.GetConnectionString("SQLiteConnection");
            //optionsBuilder.UseSqlite(connString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            // tables prefix
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes()) {
                if (entityType.GetTableName().StartsWith("AspNet", StringComparison.InvariantCultureIgnoreCase)) continue;
                entityType.SetTableName("qz" + entityType.GetTableName());
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
