using Quiz.Application.Exams;

namespace Quiz.Application.Sessions {

    public class PrepareExamSessionRequestDto : ExamQuestionsRequestDto {
        public bool ShowExamsSubSections { get; set; }
    }
}
