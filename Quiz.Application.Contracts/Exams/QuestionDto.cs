namespace PatenteN.Quiz.Domain.Exams {
    public  class QuestionDto : BaseEntityDto<Guid> {
        public int ExamId { get; set; }
        public string Statement { get; set; }
        public string? Code { get; set; }
        public string? ImageUri { get; set; }
    }
}
