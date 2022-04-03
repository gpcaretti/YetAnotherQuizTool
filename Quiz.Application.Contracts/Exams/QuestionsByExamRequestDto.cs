using PatenteN.Quiz.Application.Dtos;

namespace PatenteN.Quiz.Application.Exams {

    public class QuestionsByExamRequestDto : PagedAndSortedResultRequestDto {
        public Guid ExamId { get; set; }
        public bool IsRecursive { get; set; } = false;
        public bool IsRandom { get; set; } = false;
    }
}
