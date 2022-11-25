using Quiz.Application.Exams;
using Quiz.Application.Sessions;

namespace Quiz.Blazor.Shared.ViewModels {

    /// <summary>
    ///
    /// </summary>
    public class QuizSessionVM {
        public Guid CandidateId { get; private set; }

        public Guid? ExamId { get; private set; }
        public string? ExamName { get; private set; }
        public int ExamDuration { get; private set; }

        public IList<QuestionAndChoicesDto> Questions { get; } = new List<QuestionAndChoicesDto>();
        public IList<AnswerDetailsDto> Answers { get; } = new List<AnswerDetailsDto>();

        public int TotalQuestions => Questions.Count;
        public int TotalAnswers => Answers.Count;

        public int QuizIndex {
            get => _quizIndex;
            private set {
                _quizIndex = value;
                NotifyStateChanged();
            }
        }

        private int _quizIndex = -1;

        public bool RandomizeChoices { get; set; }
        public bool IsEnded { get; set; } = true;
        public bool IsAlreadySubmitted { get; set; }
        public bool ShowOnlyErrors { get; set; }
        public bool ShowRightChoice { get; set; }

        public event Action? OnChange;

        public QuizSessionVM(Guid candidateId) {
            CandidateId = candidateId;
        }

        // create a new user quiz session
        public void SetExam(PrepareExamSessionResponseDto input) {
            if ((input.ExamId == Guid.Empty) || ((input.Questions?.Count ?? 0) <= 0))
                throw new Exception("Please, first select and exam with a number of questions  major than zro.");

            ExamId = input.ExamId;
            ExamName = input.ExamName;
            ExamDuration = input.Duration;

            Questions.Clear();
            ((List<QuestionAndChoicesDto>)Questions).AddRange(input.Questions!);

            Answers.Clear();
            foreach (var question in Questions) {
                Answers.Add(
                    new AnswerDetailsDto {
                        ExamId = input.ExamId,
                        QuestionId = question.Id,
                        CorrectChoiceId = question.CorrectChoiceId,
                        IsMarkedAsDoubt = question.IsMarkedAsDoubt,
                        UserChoiceId = null
                    });
            }

            ResetCurrentExam();
        }

        /// <summary>
        ///     Clear all candidate's answers and set the exam from the beginning
        /// </summary>
        public void ResetCurrentExam() {
            if ((ExamId == null) || ((Questions?.Count ?? 0) <= 0)) throw new Exception("Please, first select and exam.");

            IsEnded = false;
            IsAlreadySubmitted = false;
            ShowOnlyErrors = false;
            ShowRightChoice = false;
            Answers.Where(ans => ans.UserChoiceId != null).ToList().ForEach(ans => ans.UserChoiceId = null);
            //for (var i = 0; i < Answers.Count; i++) Answers[i].UserChoiceId = null;

            QuizIndex = 0;
        }

        public QuestionAndChoicesDto? GetCurrentQuestion() => GetQuestion(QuizIndex);

        public QuestionAndChoicesDto? GetQuestion(int index) => (index >= 0) && (index < Questions.Count) ? Questions[index] : null;

        public AnswerDetailsDto? GetCurrentAnswer() => Answers.FirstOrDefault(ans => ans.QuestionId == GetCurrentQuestion()?.Id);

        public AnswerDetailsDto? GetAnswer(int index) => (index >= 0) && (index < Answers.Count) ? Answers[index] : null;

        /// <summary>
        ///     Shift of <paramref name="nShift"/> positions from the current question.
        /// </summary>
        /// <returns>The new position of this exam session. Zero if position are not valid</returns>
        public int ShiftQuestionsOf(int nShift) {
            var totalQuestions = TotalQuestions;

            if (nShift == 0) return QuizIndex;
            if (TotalQuestions <= 0) return 0;   // FIXME: gestiamo il -1?

            // calc the new position.
            // note: if 'show only errors', the new position must shift to an error. So recalculate it
            var newPos = QuizIndex + nShift;
            if (ShowOnlyErrors) {
                while ((newPos >= 0) && (newPos < totalQuestions)) {
                    var answer = Answers[newPos];
                    if (answer.IsAnswered && !answer.IsCorrect) break;
                    if (nShift < 0) newPos--; else newPos++;
                }
            }

            // if out of range, do nothing, else move to it
            return ((newPos < 0) || (newPos >= totalQuestions)) ? QuizIndex : MoveToQuestion(newPos);
        }

        /// <summary>
        ///
        /// </summary>
        public int MoveToQuestion(int position) {
            var totalQuestions = TotalQuestions;

            if (ShowOnlyErrors) {
                // go to the first/last error
                if (position <= 0) {
                    // go to the first error
                    position = ((List<AnswerDetailsDto>)Answers).FindIndex(q => q.IsAnswered && !q.IsCorrect);
                } else if (position >= totalQuestions) {
                    // go to the last error
                    position = ((List<AnswerDetailsDto>)Answers).FindLastIndex(q => q.IsAnswered && !q.IsCorrect);
                }
            }

            if (ShowRightChoice) ShowRightChoice = false;
            QuizIndex = (position < 0) ? 0 : (position >= totalQuestions) ? totalQuestions - 1 : position;
            return QuizIndex;
        }

        /// <summary>
        ///
        /// </summary>
        public bool ShowHideAnswers() {
            ShowRightChoice = !ShowRightChoice;
            return ShowRightChoice;
        }

        /// <summary>
        ///		Locally register user answer for the current question
        /// </summary>
        /// <returns>true if the record has been done, else false (e.g. exam ended, quesiton not found, etc.)</returns>
        public bool RegisterUserAnswer(Guid questionId, Guid choiceId) {
            if (IsEnded) return false;

            var answer = Answers.FirstOrDefault(ans => ans.QuestionId == questionId);
            if (answer != null) {
                answer.UserChoiceId = choiceId;
                return true;
            }

            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="isMarkedAsDoubt"></param>
        private void MarkUserAnswerAsDoubt(Guid questionId, bool? isMarkedAsDoubt = null) {
            if (IsEnded) return;
            var answer = Answers.FirstOrDefault(ans => ans.QuestionId == questionId);
            if (answer != null) {
                answer.IsMarkedAsDoubt = isMarkedAsDoubt.GetValueOrDefault();
            }
        }

        private void NotifyStateChanged() {
            OnChange?.Invoke();
        }
    }
}