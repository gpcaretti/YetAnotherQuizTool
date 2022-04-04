using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quiz.Domain.Exams.Sessions {

    /// <summary>
    ///     Store candidate's test sessions data
    /// </summary>
    public class ExamSessionItem : BaseEntity<Guid> {
        public ExamSessionItem(Guid id)
            : base(id) {
        }

        [ForeignKey(nameof(Session))]
        public Guid SessionId { get; set; }
        public ExamSession Session { get; set; }

        [ForeignKey(nameof(Question))]
        public Guid QuestionId { get; set; }
        public Question Question { get; set; }

        [ForeignKey(nameof(Choice))]
        public Guid ChoiceId { get; set; }
        public Choice Choice { get; set; }

        public bool IsCorrect { get; set; }
        public bool IsMarkedAsDoubt { get; set; } = false;
        public bool IsMarkedAsHidden { get; set; } = false;
    }
}
