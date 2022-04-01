using System.Collections.Generic;

namespace PatenteN.Quiz.Domain.Exams {
    public class QnADto {
        public int ExamID { get; set; }
        public string Exam { get; set; }
        public IList<QuestionDetailsDto> questions { get; set; }
    }

    public class QuestionDetailsDto {
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public int QuestionType { get; set; }
        public IList<OptionDetailsDto> options { get; set; }
        public AnswerDetailsDto answer { get; set; }
    }

    public class OptionDetailsDto {
        public int OptionID { get; set; }
        public string Option { get; set; }
    }
    public class AnswerDetailsDto {
        public int AnswarID { get; set; }
        public int OptionID { get; set; }
        public string Answar { get; set; }
    }

}
