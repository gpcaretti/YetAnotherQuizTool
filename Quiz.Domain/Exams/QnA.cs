using System;
using System.Collections.Generic;

namespace PatenteN.Quiz.Domain.Exams {
    public class QnA {
        public Guid ExamId { get; set; }
        public string Exam { get; set; }
        public IList<QuestionDetails> questions { get; set; }
    }
    public class QuestionDetails {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int QuestionType { get; set; }
        public IList<OptionDetails> options { get; set; }
        public AnswerDetails answer { get; set; }
    }
    public class OptionDetails {
        public Guid OptionId { get; set; }
        public string Option { get; set; }
    }
    public class AnswerDetails {
        public Guid AnswerId { get; set; }
        public Guid OptionId { get; set; }
        public string Answer { get; set; }
    }

}
