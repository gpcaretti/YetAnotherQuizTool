using System.ComponentModel.DataAnnotations;

namespace Quiz.Application.Exams {

    public class UpdateExamDto : CreateExamDto {
        public Guid Id { get; set; }
    }

    public class CreateExamDto {
        public Guid? AncestorId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Code { get; set; }

        /// <summary>Duration of the exam (0 = no duration specified)</summary>
        public int Duration { get; set; }

        public decimal FullMarks { get; set; }
    }
}
