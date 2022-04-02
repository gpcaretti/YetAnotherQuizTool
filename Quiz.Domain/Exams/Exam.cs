using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatenteN.Quiz.Domain.Exams {
    public class Exam : BaseEntity<Guid> {
        public Exam(Guid id)
            : base(id) {
        }

        [ForeignKey(nameof(Ancestor))]
        public Guid? AncestorId { get; set; }

        [MaxLength(1024)]
        public string Name { get; set; }

        [MaxLength(16)]
        public string? Code { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DefaultValue(false)]
        public decimal FullMarks { get; set; } = 100;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Duration { get; set; }

        public Exam Ancestor { get; set; }
    }
}
