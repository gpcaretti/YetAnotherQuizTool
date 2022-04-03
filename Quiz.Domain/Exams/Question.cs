using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatenteN.Quiz.Domain.Exams {

    public class Question : BaseEntity<Guid> {
        //public Question()
        //    : this(Guid.Empty) {
        //}

        public Question(Guid id)
            : base(id) {
            Choices = new HashSet<Choice>();
        }

        [ForeignKey(nameof(Exam))]
        public Guid ExamId { get; set; }
        public Exam Exam { get; set; }

        [MaxLength(16)]
        public string? Code { get; set; }

        public string Statement { get; set; }

        [MaxLength(256)]
        public string? ImageUri { get; set; }

        public virtual ICollection<Choice> Choices { get; set; }
    }
}
