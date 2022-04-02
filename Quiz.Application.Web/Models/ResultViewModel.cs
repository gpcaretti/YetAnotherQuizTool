using System.ComponentModel.DataAnnotations;

namespace PatenteN.Quiz.Application.Web.Models {
    public class ResultViewModel
    {
        [Required]
        public int CandidateId;
        [Required]
        public int ExamId;
        [Required]
        public int QuestionId;
        [Required]
        public int AnswerId;
        [Required]
        public int SelectedOption;
    }
}
