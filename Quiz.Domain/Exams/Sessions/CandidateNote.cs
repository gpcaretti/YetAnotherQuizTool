using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Quiz.Domain.Exams.Sessions {

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
        public IdentityUser<Guid> Candidate { get; set; }

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
