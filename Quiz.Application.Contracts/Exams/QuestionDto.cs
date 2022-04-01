namespace PatenteN.Quiz.Domain.Exams {
    public  class QuestionDto {
        public int QuestionID { get; set; }
        public int QuestionType { get; set; }  //MCQ-1      
        public string DisplayText { get; set; }
        public int ExamID { get; set; }
    }
}
