using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Quiz.Domain.Exams;
using Quiz.Domain.Exams.Sessions;

namespace Quiz.Domain {
    public class QuizDBContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid> {
        private const string ADMIN_ROLE_ID = "9a8dc12f-c862-e400-6e11-3a0355e4c36f";
        private const string MANAGER_ROLE_ID = "1a5442a6-de43-d043-8b50-3a0356472148";
        private const string CANDIDATE_ROLE_ID = "8d29bb6f-d47c-ac17-0256-3a035ce14eb3";

        private const string ADMIN_EMAIL = "admin@gpsoftware.it";
        private const string ADMIN_USERNAME = "admin@gpsoftware.it";
        private const string ADMIN_PASSWORD = "mypassword";
        private const string ADMIN_ID = "36bace61-5b6a-6bd6-6050-3a030665f9f7";

        public QuizDBContext() {
        }

        public QuizDBContext(DbContextOptions<QuizDBContext> options) : base(options) {
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

            DoModelCreating(modelBuilder);
            DoFeedData(modelBuilder);
        }

        private void DoModelCreating(ModelBuilder modelBuilder) {
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

        private void DoFeedData(ModelBuilder modelBuilder) {
            // roles
            modelBuilder.Entity<IdentityRole<Guid>>()
                .HasData(
                         new IdentityRole<Guid> {
                             Id = new Guid(ADMIN_ROLE_ID),
                             Name = "Admin",
                             NormalizedName = "ADMIN",
                             ConcurrencyStamp = new DateTime(2022, 04, 25).ToString(),
                         },
                         new IdentityRole<Guid> {
                             Id = new Guid(MANAGER_ROLE_ID),
                             Name = "Manager",
                             NormalizedName = "MANAGER",
                             ConcurrencyStamp = new DateTime(2022, 04, 25, 0, 0, 0, DateTimeKind.Utc).ToString(),
                         },
                         new IdentityRole<Guid> {
                             Id = new Guid(CANDIDATE_ROLE_ID),
                             Name = "Candidate",
                             NormalizedName = "CANDIDATE",
                             ConcurrencyStamp = new DateTime(2022, 04, 25, 0, 0, 0, DateTimeKind.Utc).ToString(),
                         }
                     );

            // users
            var hasher = new PasswordHasher<IdentityUser<Guid>>();
            modelBuilder.Entity<IdentityUser<Guid>>()
                .HasData(
                         new IdentityUser<Guid> {
                             Id = new Guid(ADMIN_ID),
                             Email = ADMIN_EMAIL,
                             NormalizedEmail = ADMIN_EMAIL.ToUpperInvariant(),
                             UserName = ADMIN_USERNAME,
                             NormalizedUserName = ADMIN_USERNAME.ToUpperInvariant(),
                             PasswordHash = hasher.HashPassword(null, ADMIN_PASSWORD),
                             EmailConfirmed = true,
                             SecurityStamp = new DateTime(2022, 04, 25, 0, 0, 0, DateTimeKind.Utc).ToString()
                         }
                     );

            modelBuilder.Entity<IdentityUserRole<Guid>>()
                .HasData(
                    new IdentityUserRole<Guid> {
                        RoleId = new Guid(ADMIN_ROLE_ID),
                        UserId = new Guid(ADMIN_ID)
                    }
                );
        }

    }
}
