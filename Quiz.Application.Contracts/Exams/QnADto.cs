using System.Collections.Generic;

namespace PatenteN.Quiz.Domain.Exams {
    public class QnADto {
        public Guid ExamId { get; set; }
        public string ExamName { get; set; }
        public ICollection<QuestionAndChoicesDto> Questions { get; set; }
        public int totalCount => Questions?.Count ?? 0;
    }

    public class QuestionDetailsDto {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int QuestionType { get; set; }
        public IList<OptionDetailsDto> options { get; set; }
        public AnswerDetailsDto answer { get; set; }
    }

    public class OptionDetailsDto {
        public Guid OptionId { get; set; }
        public string Option { get; set; }
    }
    public class AnswerDetailsDto {
        public Guid AnswerId { get; set; }
        public Guid OptionId { get; set; }
        public string Answer { get; set; }
    }

}
