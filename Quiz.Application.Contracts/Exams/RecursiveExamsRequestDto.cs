using System.ComponentModel.DataAnnotations;

namespace Quiz.Application.Exams {
    public class RecursiveExamsRequestDto {
        [Required]
        public Guid? ExamId { get; set; }

        [Range(0, 100)]
        public int MaxDeep { get; set; }
    }
}