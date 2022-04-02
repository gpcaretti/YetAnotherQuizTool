namespace PatenteN.Quiz.Domain.Exams {
    public class ExamDto : BaseEntityDto<Guid> {
        public Guid? AncestorId { get; set; }
        public string Name { get; set; }
        public string? Code { get; set; }
        public decimal FullMarks { get; set; }
        public decimal Duration { get; set; }

        public ExamDto? Ancestor { get; set; }
    }
}
