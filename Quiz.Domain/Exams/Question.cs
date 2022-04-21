using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quiz.Domain.Exams {

    public class Question : Entity<Guid> {

        // only for EF
        private Question() {
        }

        public Question(Guid id)
            : base(id) {
            Choices = new HashSet<Choice>();
        }

        [ForeignKey(nameof(Exam))]
        public Guid ExamId { get; set; }
        public Exam Exam { get; set; }

        [MaxLength(16)]
        public string? Code { get; set; }

        public int? Position { get; set; }

        public string Statement { get; set; }

        [MaxLength(256)]
        public string? ImageUri { get; set; }

        public virtual ICollection<Choice> Choices { get; private set; }
    }
}
