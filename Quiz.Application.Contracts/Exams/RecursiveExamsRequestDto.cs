using System.ComponentModel.DataAnnotations;

namespace Quiz.Application.Exams {
    public class RecursiveExamsRequestDto {

		//public RecursiveExamsRequestDto() {
		//}

		public RecursiveExamsRequestDto(Guid? examId, int maxDeep = 10) {
			ExamId = examId;
			MaxDeep = maxDeep;
		}

		[Required]
        public Guid? ExamId { get; set; }

        [Range(0, 100)]
        public int MaxDeep { get; set; }
    }
}