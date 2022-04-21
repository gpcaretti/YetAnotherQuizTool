namespace Quiz.Application.Exams.Sessions {

    public class ExamSessionResultsRequestDto {
        public Guid CandidateId { get; set; }
        public Guid ExamId { get; set; }
        public bool IsEnded { get; set; }
        public bool SkipUnanswered { get; set; } = true;
        public IList<AnswerDetailsDto> Answers { get; set; }
    }

    public class AnswerDetailsDto {
        public Guid ExamId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid CorrectChoiceId { get; set; }
        public Guid? UserChoiceId { get; set; }
        public bool IsCorrect => (UserChoiceId != null) && (UserChoiceId == CorrectChoiceId);
        public bool IsMarkedAsHidden { get; set; }
        public bool IsMarkedAsDoubt { get; set; }
    }

}
