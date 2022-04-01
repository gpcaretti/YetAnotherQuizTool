using Microsoft.EntityFrameworkCore;
using PatenteN.Quiz.Domain.Exams;
using PatenteN.Quiz.Domain.Users;

namespace PatenteN.Quiz.Domain {
    public class QuizDBContext : DbContext
    {
        public QuizDBContext()
        {
        }
        public QuizDBContext(DbContextOptions<QuizDBContext> options) : base(options)
        {
        }
        public virtual DbSet<Candidate> Candidate { get; set; }
        public virtual DbSet<Exam> Exam { get; set; }
        public virtual DbSet<Question> Question { get; set; }
        public virtual DbSet<Choice> Choice { get; set; }
        public virtual DbSet<Answer> Answer { get; set; }
        public virtual DbSet<Result> Result { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuizAttempt>(eb =>
            {
                eb.HasNoKey();
                eb.ToView(null);
            });

            modelBuilder.Entity<QuizReport>(eb =>
            {
                eb.HasNoKey();
                eb.ToView(null);
            });
        }
    }
}
