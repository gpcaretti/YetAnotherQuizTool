using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quiz.Domain.Exams {

    public class Question : Entity<Guid> {

        // only for EF
        private Question() {
        }

        public Question(Guid id, Guid examId)
            : base(id) {
            Choices = new HashSet<Choice>();
            ExamId = examId;
        }

        [ForeignKey(nameof(Exam))]
        public Guid ExamId { get; private set; }
        public Exam Exam { get; private set; }

        [MaxLength(16)]
        public string? Code { get; set; }

        public int? Position { get; set; }

        public string Statement { get; set; }

        [MaxLength(256)]
        public string? ImageUri { get; set; }

        public virtual ICollection<Choice> Choices { get; private set; }
    }
}
