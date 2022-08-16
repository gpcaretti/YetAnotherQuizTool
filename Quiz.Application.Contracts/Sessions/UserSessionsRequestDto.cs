    using System.ComponentModel.DataAnnotations;

namespace Quiz.Application.Sessions {
    public class UserSessionsRequestDto {

        [Required]
        public Guid? CandidateId { get; set; }
        public Guid? ExamId { get; set; }

        [Range(1, 20)]
        public int MaxDeep { get; set; } = 10;
    }
}
