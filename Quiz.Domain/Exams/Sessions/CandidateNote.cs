using System.ComponentModel.DataAnnotations.Schema;

namespace Quiz.Domain.Exams.Sessions {

    /// <summary>
    ///     Have multiple key (CandidateId, QuestionId) 
    /// </summary>
    public class CandidateNote : Entity {

        // only for EF
        private CandidateNote() {
        }

        public CandidateNote(Guid candidateId, Guid questionId, Guid examId) {
            this.CandidateId = candidateId.ToString();
            this.QuestionId = questionId;
            this.ExamId = examId;
        }

        //[ForeignKey(nameof(Candidate))]   // <= commented out as not working using 2 separated DbContext
        public string CandidateId { get; private set; }
        //public ApplicationUser Candidate { get; private set; }    // <= commented out as not working using 2 separated DbContext

        [ForeignKey(nameof(Question))]
        public Guid QuestionId { get; private set; }
        public Question Question { get; private set; }

        public Guid ExamId { get; private set; }

        public int NumOfWrongAnswers { get; private set; } = 0;

        public int AddErrorCount() {
            NumOfWrongAnswers += 2;     // each error counts as double
            return NumOfWrongAnswers;
        }

        public int SubErrorCount() {
            if (--NumOfWrongAnswers < 0) NumOfWrongAnswers = 0;
            return NumOfWrongAnswers;
        }

        public bool IsMarkedAsDoubt { get; set; } = false;
        public bool IsMarkedAsHidden { get; set; } = false;

        public string? Note { get; set; }
    }
}
