namespace Quiz.Application.Exams {

    public class PrepareExamSessionResponseDto {
        public Guid ExamId { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public ICollection<QuestionAndChoicesDto> Questions { get; set; }
        public int TotalCount => Questions?.Count ?? 0;
    }

}
