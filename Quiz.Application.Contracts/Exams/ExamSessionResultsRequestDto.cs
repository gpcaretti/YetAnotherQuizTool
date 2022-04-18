namespace Quiz.Application.Exams {

    public class ExamSessionResultsRequestDto {
        public Guid ExamId { get; set; }
        public bool IsEnded { get; set; }
        public IList<AnswerDetailsDto> Answers { get; set; }
    }

    public class AnswerDetailsDto {
        public Guid QuestionId { get; set; }
        public Guid CorrectChoiceId { get; set; }
        public Guid? UserChoiceId { get; set; }
        public bool IsCorrect => (UserChoiceId != null) && (UserChoiceId == CorrectChoiceId);
    }

    [Obsolete()]
    public class QuestionDetailsDto {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int QuestionType { get; set; }
        public IList<OptionDetailsDto> options { get; set; }
        public AnswerDetailsDto answer { get; set; }
    }

    [Obsolete()]
    public class OptionDetailsDto {
        public Guid OptionId { get; set; }
        public string Option { get; set; }
    }

}
