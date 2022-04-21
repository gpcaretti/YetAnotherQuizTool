using Quiz.Application.Dtos;

namespace Quiz.Application.Exams.Sessions {

    public class PrepareExamSessionRequestDto : PagedAndSortedResultRequestDto {
        public Guid ExamId { get; set; }
        public bool IsRecursive { get; set; } = false;
        public bool IsRandom { get; set; } = false;
        public bool OnlyErrorOrDoubt { get; set; }
    }
}
