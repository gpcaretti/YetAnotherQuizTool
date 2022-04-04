using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quiz.Domain.Exams {

    public class Choice : BaseEntity<Guid> {
        public Choice(Guid id)
            : base(id) {
        }

        [ForeignKey(nameof(Question))]
        public Guid QuestionId { get; set; }
        public Question Question { get; set; }

        public string Statement { get; set; }

        /// <summary>true if the right answer</summary>
        public bool IsCorrect { get; set; }

        /// <summary>Relative position of the choice (from 0)</summary>
        public int? Position { get; set; }
    }
}
