using System.ComponentModel.DataAnnotations.Schema;
using Quiz.Domain.Users;

namespace Quiz.Domain.Exams {

    /// <summary>
    ///     Have multiple key (CandidateId, QuestionId) 
    /// </summary>
    public class CandidateNote : Entity {

        // only for EF
        private CandidateNote() {
        }

        public CandidateNote(Guid candidateId, Guid questionId, Guid examId) {
            this.CandidateId = candidateId;
            this.QuestionId = questionId;
            this.ExamId = examId;
        }

        [ForeignKey(nameof(Candidate))]
        public Guid CandidateId { get; set; }
        public Candidate Candidate { get; set; }

        [ForeignKey(nameof(Question))]
        public Guid QuestionId { get; set; }
        public Question Question { get; set; }

        public Guid ExamId { get; private set; }

        public int NumOfWrongAnswers { get; private set; } = 0;

        public int AddErrorCount() {
            NumOfWrongAnswers += 2;
            return NumOfWrongAnswers;
        }

        public int SubErrorCount() {
            if (--NumOfWrongAnswers < 0) NumOfWrongAnswers = 0;
            return NumOfWrongAnswers;
        }

        public bool IsMarkedAsDoubt { get; set; } = false;
        public bool IsMarkedAsHidden { get; set; } = false;

        public string Note { get; set; }
    }
}
