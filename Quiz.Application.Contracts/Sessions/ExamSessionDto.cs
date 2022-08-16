using System.ComponentModel.DataAnnotations;

namespace Quiz.Application.Sessions {

    /// <summary>
    /// 
    /// </summary>
    public class ExamSessionDto : BaseEntityDto<Guid> {
        public Guid? CandidateId { get; set; }

        [Required]
        public Guid? ExamId { get; set; }

        public string ExamName { get; set; }

        public bool IsEnded { get; set; }

        public DateTimeOffset ExecutedOn { get; set; }

        /// <summary>A CSV string of the sequence of questions for this exam session</summary>
        public string QSequence { get; set; }

        public int NumOfQuestions { get; set; }
        public int NumOfCorrectAnswers { get; set; }
        public int NumOfWrongAnswers { get; set; }
        public int NumOfNotAnswered => NumOfQuestions - NumOfCorrectAnswers - NumOfWrongAnswers;
    }

    public class ExamSessionWithAnswersDto : ExamSessionDto {
        public IList<AnswerDetailsDto> Answers { get; set; }

        public int TotalCount => Answers?.Count ?? 0;

        public AnswerDetailsDto this[int index] => Answers[index];
    }

    /// <summary>
    /// 
    /// </summary>
    public class AnswerDetailsDto {
        [Required]
        public Guid? ExamId { get; set; }

        [Required]
        public Guid? QuestionId { get; set; }

        [Required]
        public Guid CorrectChoiceId { get; set; }

        public Guid? UserChoiceId { get; set; }
        public bool IsAnswered => (UserChoiceId != null);
        public bool IsCorrect => (UserChoiceId != null) && (UserChoiceId == CorrectChoiceId);
        public bool IsMarkedAsHidden { get; set; }
        public bool IsMarkedAsDoubt { get; set; }
    }

}
