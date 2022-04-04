namespace Quiz.Domain.Exams {
    public class QuestionDto : BaseEntityDto<Guid> {
        public Guid ExamId { get; set; }
        public string Statement { get; set; }
        public string? Code { get; set; }
        public string? ImageUri { get; set; }
    }

    public class QuestionAndChoicesDto : QuestionDto {
        public IList<ChoiceDto> Choices { get; set; }
        public Guid CorrectChoiceId => Choices?[CorrectChoiceIdx].Id ?? Guid.Empty;

        public int CorrectChoiceIdx {
            get {
                ChoiceDto? dto = Choices?.Where(ch => ch.IsCorrect).FirstOrDefault();
                return (dto != null) ? Choices.IndexOf(dto) : -1;
            }
        }
    }
}
