using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Quiz.Domain.Exams.Sessions {

    /// <summary>
    ///     Store candidate's test sessions
    /// </summary>
    public class ExamSession : Entity<Guid> {

        // only for EF
        private ExamSession() {
        }

        public ExamSession(Guid id)
            : base(id) {
        }

        [ForeignKey(nameof(Candidate))]
        public Guid CandidateId { get; set; }
        public IdentityUser<Guid> Candidate { get; set; }

        [ForeignKey(nameof(Exam))]
        public Guid ExamId { get; set; }
        public Exam? Exam { get; set; }

        /// <summary>When true, the session should be read-only</summary>
        public bool IsEnded { get; set; } = false;

        [MaxLength(1024)]
        public string ExamName { get; set; }

        public DateTimeOffset ExecutedOn { get; set; }

        /// <summary>A CSV string of the sequence of questions for this exam session</summary>
        public string QSequence { get; set; }

        public int NumOfQuestions { get; set; }
        public int NumOfCorrectAnswers { get; set; }
        public int NumOfWrongAnswers { get; set; }
        public int NumOfNotAnswered => NumOfQuestions - NumOfCorrectAnswers - NumOfWrongAnswers;

        [Column(TypeName = "decimal(18,2)")]
        [DefaultValue(false)]
        public decimal? Marks { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DefaultValue(false)]
        public decimal? FullMarks { get; set; }
    }
}
