using System;
using System.ComponentModel.DataAnnotations;

namespace PatenteN.Quiz.Domain.Exams {
    public class Result : BaseEntity<Guid> {

        public Result()
            : this(Guid.Empty) {
        }

        public Result(Guid id)
            : base(id) {
        }

        [MaxLength(1024)]
        public string SessionId { get; set; }
        public Guid CandidateId { get; set; }
        public Guid ExamId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid AnswerId { get; set; }
        public Guid SelectedOptionId { get; set; }
        public bool IsCorrent { get; set; }
    }
}
