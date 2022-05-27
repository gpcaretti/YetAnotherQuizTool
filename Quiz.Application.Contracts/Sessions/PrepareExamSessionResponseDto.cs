using Quiz.Application.Exams;

namespace Quiz.Application.Sessions {

    public class PrepareExamSessionResponseDto {
        public Guid ExamId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Duration { get; set; }
        public IList<QuestionAndChoicesDto> Questions { get; set; }
        public bool RandomizeChoices { get; set; }
        public int TotalCount => Questions?.Count ?? 0;
    }

}
