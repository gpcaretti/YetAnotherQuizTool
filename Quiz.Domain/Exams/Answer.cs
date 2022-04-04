using System;

namespace Quiz.Domain.Exams {

    public class Answer : BaseEntity<Guid> {
        public Answer()
            : this(Guid.Empty) {
        }

        public Answer(Guid id)
            : base(id) {
        }

        public Guid QuestionId { get; set; }
        public Guid ChoiceId { get; set; }
        public string Statement { get; set; }
    }
}
