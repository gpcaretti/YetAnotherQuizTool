using System.ComponentModel.DataAnnotations;
using Quiz.Application.Dtos;

namespace Quiz.Application.Exams {

    public class PrepareExamSessionRequestDto : PagedAndSortedResultRequestDto {
        public Guid ExamId { get; set; }
        public bool IsRecursive { get; set; } = false;
        public bool IsRandom { get; set; } = false;
    }
}
