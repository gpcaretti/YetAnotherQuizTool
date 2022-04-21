using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public Guid CandidateId { get; set; }

        [ForeignKey(nameof(Exam))]
        public Guid ExamId { get; set; }
        public Exam? Exam { get; set; }

        [MaxLength(1024)]
        public string ExamName { get; set; }

        public DateTimeOffset ExecutedOn { get; set; }

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
