using System.ComponentModel.DataAnnotations;

namespace Quiz.Application.Sessions {

    /// <summary>
    /// 
    /// </summary>
    public class ExamSessionResultsRequestDto {
        public Guid CandidateId { get; set; }
        public Guid ExamId { get; set; }
        public bool IsEnded { get; set; }
        public bool SkipUnanswered { get; set; } = true;
        public IList<AnswerDetailsDto> Answers { get; set; }

        public int TotalCount => Answers?.Count ?? 0;
        public AnswerDetailsDto this[int index] => Answers[index];
    }

    /// <summary>
    /// 
    /// </summary>
    public class AnswerDetailsDto {
        [Required]
        public Guid ExamId { get; set; }

        [Required]
        public Guid QuestionId { get; set; }

        [Required]
        public Guid CorrectChoiceId { get; set; }

        public Guid? UserChoiceId { get; set; }
        public bool IsAnswered => (UserChoiceId != null);
        public bool IsCorrect => (UserChoiceId != null) && (UserChoiceId == CorrectChoiceId);
        public bool IsMarkedAsHidden { get; set; }
        public bool IsMarkedAsDoubt { get; set; }
    }

}
