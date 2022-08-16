using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Quiz.Application.Dtos;
using Quiz.Application.Exams;
using Quiz.Application.Sessions;
using Quiz.Domain.Identity;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace Quiz.Blazor.ServerApp.Pages {

    public partial class QuizViewer : Microsoft.AspNetCore.Components.ComponentBase {

        [Parameter]
        [SupplyParameterFromQuery(Name = "sessionId")]
        public Guid OldSessionId { get; set; } = Guid.Empty;

        //[Parameter]
        //public string? SessionId { get; set; } = null;

        protected ApplicationUser? User;
        protected QuizSession? ExamSession;

        protected PrepareExamSessionRequestDto? NewQuizModel;
        protected ICollection<ExamDto>? AvailableExams;
        protected bool ShowExamsSubSection = false;

        protected int WaitingCnt;

        private EditContext? editContext;

        protected override async Task OnInitializedAsync() {
            WaitingCnt = 1;
            try {
                await base.OnInitializedAsync();

                // get the current user
                AuthenticationState authState = await GetAuthenticationStateAsync.GetAuthenticationStateAsync();
                User = await UserManager.GetUserAsync(authState.User);

                //NavManager.TryGetQueryString<int>("initialCount", out currentCount);
                HandleReset();

                // get available exams
                AvailableExams = await _examAppService.GetAll(new PagedAndSortedResultRequestDto { MaxResultCount = 100, Sorting = nameof(ExamDto.Code) });
                NewQuizModel!.ExamId = AvailableExams.FirstOrDefault()?.Id;
                //editContext.OnValidationRequested += HandleValidationRequested;

                if (OldSessionId != Guid.Empty) {
                    // get a set of quiz according to user selections/options
                    PrepareExamSessionResponseDto output = await _examSessionAppService.PrepareExamSession(OldSessionId);
                    if (output.TotalCount <= 0) {
                        //await JsRuntime.InvokeVoidAsync("alert", "No available question for the selected exam and options"); // Alert
                        return;
                    }
                    ExamSession.SetExam(output);

                    NewQuizModel!.ExamId = AvailableExams.FirstOrDefault(ex => ex.Id == output.ExamId)?.Id ?? AvailableExams.FirstOrDefault()?.Id;
                    NewQuizModel!.MaxResultCount = output.TotalCount;

                    // move to first question and return
                    MoveToQuestion(0);
                }
            } finally {
                if (WaitingCnt > 0) WaitingCnt--;
            }
        }

        /// <summary>
        ///		UI request for a new session
        /// </summary>
        private async Task StartExamSessionClick(MouseEventArgs evt) {
            // if no exam selected, warn and return
            if (!NewQuizModel!.ExamId.HasValue) {
                await JsRuntime.InvokeVoidAsync("alert", "Please, select an exam"); // Alert
                return;
            }

            WaitingCnt++;
            try {
                // get a set of quiz according to user selections/options
                NewQuizModel.IsRecursive = true;
                var output = await _examSessionAppService.PrepareExamSession(NewQuizModel);
                if (output.TotalCount <= 0) {
                    //await JsRuntime.InvokeVoidAsync("alert", "No available question for the selected exam and options"); // Alert
                    return;
                }

                ExamSession.SetExam(output);

                // move to first question and return
                MoveToQuestion(0);
            } finally {
                if (WaitingCnt > 0) WaitingCnt--;
            }
        }

        /// <summary>
        ///		UI request for restarting the current session
        /// </summary>
        private async Task RestartExamSessionClick(MouseEventArgs evt) {
            if ((ExamSession == null) || (ExamSession.TotalAnswers <= 0)) {
                await JsRuntime.InvokeVoidAsync("alert", "Oops! There is no user session to restart."); // Alert
                return;
            }

            bool confirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Do you want to restart current exam session?"); // Confirm
            if (!confirmed) return;

            ExamSession.ResetCurrentExam();
        }

        private void HandleReset() {
            try {
                ExamSession = new QuizSession(Guid.Parse(User.Id));

                NewQuizModel = new PrepareExamSessionRequestDto {
                    CandidateId = ExamSession.CandidateId,
                    IsRandom = true,
                    MaxResultCount = 20,    // FIXME: how to define the default num of questions per exam?
                };
                editContext = new EditContext(NewQuizModel);
            }
            finally {
            }
        }

        /// <summary>
        ///     Shift of <paramref name="nShift"/> positions from the current question.
        /// </summary>
        /// <param name="nShift"></param>
        private void ShiftQuestionsOf(int nShift) {
            if ((ExamSession == null) || (nShift == 0)) return;

            var totalQuestions = ExamSession.TotalQuestions;

            // calc the new position.
            // note: if 'show only errors', the new position must shift to an error. So recalculate it
            var newPos = ExamSession.QuizIndex + nShift;
            if (ExamSession.ShowOnlyErrors) {
                while ((newPos >= 0) && (newPos < totalQuestions)) {
                    var answer = ExamSession.Answers[newPos];
                    if (answer.IsAnswered && !answer.IsCorrect) break;
                    if (nShift < 0) newPos--; else newPos++;
                }
            }

            // if out of range, do nothing
            if ((newPos < 0) || (newPos >= totalQuestions)) return;

            MoveToQuestion(newPos);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        private void MoveToQuestion(int position) {
            if (ExamSession == null) return;

            var totalQuestions = ExamSession.TotalQuestions;

            if (ExamSession.ShowOnlyErrors) {
                // go to the first/last error
                if (position <= 0) {
                    // go to the first error
                    position = ExamSession.Answers.ToList().FindIndex(q => q.IsAnswered && !q.IsCorrect);
                } else if (position >= totalQuestions) {
                    // go to the last error
                    position = ExamSession.Answers.ToList().FindLastIndex(q => q.IsAnswered && !q.IsCorrect);
                }
            }

            if (ExamSession.ShowRightChoice) ExamSession.ShowRightChoice = false;
            ExamSession.QuizIndex = (position < 0) ? 0 : (position >= totalQuestions) ? totalQuestions - 1 : position;
        }

        // Locally register user answer for the current question
        private void RegisterUserAnswer(Guid questionId, Guid choiceId) {
            if ((ExamSession == null) || ExamSession.IsEnded) return;
            var answer = ExamSession?.Answers?.FirstOrDefault(ans => ans.QuestionId == questionId);
            if (answer != null) {
                answer.UserChoiceId = choiceId;
            }
        }

        private void MarkUserAnswerAsDoubt(Guid questionId, bool? isMarkedAsDoubt = null) {
            if ((ExamSession == null) || ExamSession.IsEnded) return;
            var answer = ExamSession?.Answers?.FirstOrDefault(ans => ans.QuestionId == questionId);
            if (answer != null) {
                answer.IsMarkedAsDoubt = isMarkedAsDoubt.GetValueOrDefault();
            }
        }

        private void ShowHideAnswers(MouseEventArgs evt) {
            if (ExamSession != null) ExamSession.ShowRightChoice = !ExamSession.ShowRightChoice;
        }

        private void ShowOnlyErrorsToggle() {
            if (ExamSession != null) {
                ExamSession.ShowOnlyErrors = !ExamSession.ShowOnlyErrors;
                if (ExamSession.ShowOnlyErrors) MoveToQuestion(0);
            }
        }

        private void EndExamSession(MouseEventArgs evt) {
            if (ExamSession != null) ExamSession.IsEnded = true;
            MoveToQuestion(0);
        }

        private async Task EndAndSubmitExamSession(MouseEventArgs evt) {
            if ((ExamSession == null) || (ExamSession.TotalAnswers <= 0)) {
                await JsRuntime.InvokeVoidAsync("alert", "Oops! There is no user session to save."); // Alert
                return;
            } else if (ExamSession.IsAlreadySubmitted) {
                await JsRuntime.InvokeVoidAsync("alert", "Oops! Exam session has already been submitted."); // Alert
                return;
            }

            bool confirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Do you want to terminate and register your quiz session?"); // Confirm
            if (!confirmed) return;

            WaitingCnt++;
            try {
                EndExamSession(evt);
                var sessionId = await _examSessionAppService.SaveUserSession(
                    new ExamSessionRequestDto {
                        CandidateId = ExamSession.CandidateId,
                        ExamId = ExamSession.ExamId,
                        IsEnded = ExamSession.IsEnded,
                        Answers = ExamSession.Answers
                    });
                ExamSession.IsAlreadySubmitted = true;

                if (WaitingCnt > 0) WaitingCnt--;
                //await JsRuntime.InvokeVoidAsync("alert", "Your session has been saved. Good luck!");
            } catch (Exception ex) {
                if (WaitingCnt > 0) WaitingCnt--;
                _logger.LogError(ex, ex.Message);
                await JsRuntime.InvokeVoidAsync("alert", "Oops! An error occurred trying to execute the requested operation: " + ex.Message);
            } finally {
            }

        }

        private void HandleValidSubmit() {
            // TODO
            var modelJson = JsonSerializer.Serialize(NewQuizModel, new JsonSerializerOptions { WriteIndented = true });
            //JSRuntime.InvokeVoidAsync("alert", $"SUCCESS!! :-)\n\n{modelJson}");
        }

        /// <summary>
        /// 
        /// </summary>
        protected class QuizSession {

            private IList<QuestionAndChoicesDto> _questions;

            public QuizSession(Guid candidateId) {
                CandidateId = candidateId;
                _questions = new List<QuestionAndChoicesDto>(0);
                Answers = new List<AnswerDetailsDto>();
            }

            [Required]
            public Guid CandidateId { get; private set; }

            [Required]
            public Guid? ExamId { get; private set; }
            public string? ExamName { get; private set; }
            public int ExamDuration { get; private set; }
            public IList<QuestionAndChoicesDto> Questions {
                get {
                    //// if user want to see only errors, filter the questions
                    //if (ShowOnlyErrors) {
                    //    var wrongIds = Answers.Where(ans => ans.IsAnswered && !ans.IsCorrect).Select(ans => ans.QuestionId);
                    //    return _questions
                    //            .Where(q => wrongIds.Contains(q.Id))
                    //            .ToList();
                    //} else {
                    //    return _questions;
                    //}
                    return _questions;
                }
                private set => _questions = value;
            }

            public int TotalQuestions => Questions?.Count ?? 0;
            public bool RandomizeChoices { get; set; }

            public int QuizIndex { get; set; }
            public bool IsEnded { get; set; } = true;
            public bool IsAlreadySubmitted { get; set; }

            public IList<AnswerDetailsDto> Answers { get; private set; }
            public int TotalAnswers => Answers?.Count ?? 0;

            public bool ShowOnlyErrors { get; set; }
            public bool ShowRightChoice { get; set; }

            // create a new user quiz session 
            public void SetExam(PrepareExamSessionResponseDto input) {
                ExamId = input.ExamId;
                ExamName = input.ExamName;
                ExamDuration = input.Duration;
                Questions = input.Questions;

                Answers = new AnswerDetailsDto[TotalQuestions];
                for (var i = 0; i < Answers.Count; i++) {
                    var question = Questions[i];
                    Answers[i] = new AnswerDetailsDto {
                        ExamId = input.ExamId,
                        QuestionId = question.Id,
                        CorrectChoiceId = question.CorrectChoiceId,
                        IsMarkedAsDoubt = question.IsMarkedAsDoubt,
                    };
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

                QuizIndex = 0;

                for (var i = 0; i < Answers.Count; i++) {
                    Answers[i].UserChoiceId = null;
                }

            }

            public QuestionAndChoicesDto GetCurrentQuestion() => GetQuestion(QuizIndex);
            public QuestionAndChoicesDto GetQuestion(int index) => Questions[index];

            public AnswerDetailsDto? GetCurrentAnswer() => Answers.FirstOrDefault(ans => ans.QuestionId == GetCurrentQuestion().Id);
            public AnswerDetailsDto? GetAnswer(int index) => Answers[index];
        }

    }

}
