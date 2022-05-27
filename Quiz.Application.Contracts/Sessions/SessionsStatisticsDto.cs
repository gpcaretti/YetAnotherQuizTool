namespace Quiz.Application.Sessions {
    public class SessionsStatisticsDto {
        public Guid? CandidateId { get; set; }
        public string? CandidateName { get; set; }
        public string ExamName { get; set; }
        public int NumOfCarriedOutSessions { get; set; }
        public int NumOfWrongAnswers { get; set; }
        public int NumOfNeverAnswered { get; set; }
        public int NumOfDoubtAnswers { get; set; }
        public int NumOfAvailableQuestions { get; set; }
    }
}
