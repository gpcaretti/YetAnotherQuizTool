namespace Quiz.Domain.Exams {

    [Obsolete()]
    public class ResultDto : BaseEntityDto<Guid> {
        public string SessionId { get; set; }
        public Guid CandidateId { get; set; }
        public Guid ExamId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid AnswerId { get; set; }
        public Guid SelectedOptionId { get; set; }
        public bool IsCorrent { get; set; }
    }
}
