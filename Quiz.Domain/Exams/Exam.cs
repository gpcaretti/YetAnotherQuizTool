﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quiz.Domain.Exams {
    public class Exam : BaseEntity<Guid> {
        public Exam(Guid id)
            : base(id) {
        }

        [ForeignKey(nameof(Ancestor))]
        public Guid? AncestorId { get; set; }
        public Exam? Ancestor { get; set; }

        [MaxLength(1024)]
        public string Name { get; set; }

        [MaxLength(16)]
        public string? Code { get; set; }

        /// <summary>Duration of the exam (0 = no duration specifie </summary>
        [Range(0, 1440)]
        public int Duration { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        [DefaultValue(false)]
        public decimal FullMarks { get; set; } = 100;
    }
}
