using System.ComponentModel.DataAnnotations.Schema;

namespace Quiz.Domain.Exams.Sessions {

    /// <summary>
    ///     Store candidate's test sessions data
    /// </summary>
    public class ExamSessionItem : Entity<Guid> {

        // only for EF
        private ExamSessionItem() {
        }

        public ExamSessionItem(Guid id, Guid sessionId)
            : base(id) {
            SessionId = sessionId;
        }

        [ForeignKey(nameof(Session))]
        public Guid SessionId { get; private set; }
        public ExamSession Session { get; set; }

        /// <summary>Original question Id</summary>
        public Guid QuestionId { get; set; }

        /// <summary>True if answer was correct</summary>
        public bool IsCorrect { get; set; }
        public bool IsMarkedAsDoubt { get; set; } = false;
        public bool IsMarkedAsHidden { get; set; } = false;
    }
}
