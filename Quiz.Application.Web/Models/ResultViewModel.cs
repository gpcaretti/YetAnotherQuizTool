using System.ComponentModel.DataAnnotations;

namespace PatenteN.Quiz.Application.Web.Models {
    public class ResultViewModel
    {
        [Required]
        public int CandidateID;
        [Required]
        public int ExamID;
        [Required]
        public int QuestionID;
        [Required]
        public int AnswerID;
        [Required]
        public int SelectedOption;
    }
}
