using Quiz.Application;

namespace Quiz.Application.Exams {
    public class ExamDto : BaseEntityDto<Guid> {
        public Guid? AncestorId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        /// <summary>Duration of the exam (0 = no duration specified)</summary>
        public int Duration { get; set; }

        public decimal FullMarks { get; set; }

        public ExamDto Ancestor { get; set; }
    }
}
