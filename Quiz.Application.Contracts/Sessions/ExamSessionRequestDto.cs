using System.ComponentModel.DataAnnotations;

namespace Quiz.Application.Sessions {

    /// <summary>
    /// 
    /// </summary>
    public class ExamSessionRequestDto {
        [Required]
        public Guid? CandidateId { get; set; }

        [Required]
        public Guid? ExamId { get; set; }

        public bool IsEnded { get; set; }
        public bool SkipUnanswered { get; set; } = true;
        public IList<AnswerDetailsDto> Answers { get; set; }

        public int TotalCount => Answers?.Count ?? 0;
        public AnswerDetailsDto this[int index] => Answers[index];
    }

}
