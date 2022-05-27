using System.ComponentModel.DataAnnotations;
using Quiz.Application.Dtos;

namespace Quiz.Application.Exams {

    public class ExamQuestionsRequestDto : PagedAndSortedResultRequestDto {

        [Required]
        public Guid? CandidateId { get; set; }

        [Required]
        public Guid? ExamId { get; set; }

        public bool IsRecursive { get; set; } = false;

        public bool IsRandom {
            get => _isRandom;
            set { /*if (!value) OnlyErrorOrDoubt = false;*/ _isRandom = value; }
        }

        /// <summary>
        ///     if true, <see cref="OnlyNew"/> must be false
        /// </summary>
        public bool OnlyErrorOrDoubt {
            get => _onlyErrorOrDoubt;
            set { if (value) OnlyNew = false; _onlyErrorOrDoubt = value; }
        }

        /// <summary>
        ///     if true, <see cref="OnlyErrorOrDoubt"/> must be false
        /// </summary>
        public bool OnlyNew {
            get => _onlyNew;
            set { if (value) OnlyErrorOrDoubt = false; _onlyNew = value; }
        }

        private bool _isRandom = false;
        private bool _onlyErrorOrDoubt = false;
        private bool _onlyNew = false;
    }
}
