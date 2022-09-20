using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Quiz.Application.Sessions;
using Quiz.Application.Users;
using Quiz.Blazor.Shared.ViewModels;

namespace Quiz.Blazor.Maui.Standalone.Pages {

    public partial class QuizRunner {

        [Parameter]
        [SupplyParameterFromQuery(Name = "sessionId")]
        public Guid? OldSessionId { get; set; } = null;

        [CascadingParameter]
        public IModalService BlazoredModal { get; set; } = default !;

        //[Parameter]
        //public string? SessionId { get; set; } = null;

        PrepareExamSessionRequestDto ExamSessionRequest { get; set; } = new PrepareExamSessionRequestDto();

        protected CandidateDto? User;
        protected QuizSessionVM? ExamSession;

        protected int WaitingCnt;

        [Inject] protected ICandidateAppService _candidateAppService { get; set; }
        [Inject] protected IExamSessionAppService _examSessionAppService { get; set; }
        //[Inject] protected IJSRuntime JsRuntime { get; set; }
        [Inject] protected ILogger<QuizRunner> _logger { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync() {
            WaitingCnt = 1;
            try {
                // get the current user
                User = await _candidateAppService.GetCurrentUser();

                //NavManager.TryGetQueryString<int>("initialCount", out currentCount);
                HandleReset();

                //if (OldSessionId != Guid.Empty) {
                //    // get a set of quiz according to user selections/options
                //    PrepareExamSessionResponseDto output = await _examSessionAppService.PrepareExamSession(OldSessionId);
                //    if (output.TotalCount <= 0) {
                //        //await JsRuntime.InvokeVoidAsync("alert", "No available question for the selected exam and options"); // Alert
                //        BlazoredModal.Show<Quiz.Blazor.Shared.DisplayMessage>("No available question for the selected exam and options");
                //        return;
                //    }
                //    ExamSession.SetExam(output);

                //    PrepareExamSessionInput2!.ExamId = AvailableExams.FirstOrDefault(ex => ex.Id == output.ExamId)?.Id ?? AvailableExams.FirstOrDefault()?.Id;
                //    PrepareExamSessionInput2!.MaxResultCount = output.TotalCount;

                //    // move to first question and return
                //    MoveToQuestion(0);
                //}

                await base.OnInitializedAsync();
            }
            finally {
                if (WaitingCnt > 0) WaitingCnt--;
            }
        }

        /// <summary>
        ///		UI request for a new session
        /// </summary>
        protected async Task PrepareExamSessionSubmittedHandler(PrepareExamSessionRequestDto input) {
            // if no exam selected, warn and return
            if (!input.ExamId.HasValue) {
                BlazoredModal.Show<Quiz.Blazor.Shared.DisplayMessage>("Please, select an exam");
                return;
            }

            WaitingCnt++;
            try {
                // get a set of quiz according to user selections/options
                var output = await _examSessionAppService.PrepareExamSession(input);
                if (output.TotalCount <= 0) {
                    BlazoredModal.Show<Quiz.Blazor.Shared.DisplayMessage>("No available question for the selected exam and options");
                    return;
                }

                ExamSession!.SetExam(output);

                // move to first question and return
                ExamSession.MoveToQuestion(0);
            } finally {
                if (WaitingCnt > 0) WaitingCnt--;
            }
        }

        /// <summary>
        ///		UI request for restarting the current session
        /// </summary>
        private async Task RestartExamSessionClick(MouseEventArgs evt) {
            if ((ExamSession == null) || (ExamSession.TotalAnswers <= 0)) {
                BlazoredModal.Show<Quiz.Blazor.Shared.DisplayMessage>("Oops! There is no user session to restart");
                return;
            }

            var confirm = await BlazoredModal.Show<Quiz.Blazor.Shared.Confirm>(
                "Do you want to restart current exam session?",
                new ModalOptions { Position = ModalPosition.Middle, AnimationType = ModalAnimationType.FadeInOut }
                ).Result;
            if (!confirm.Confirmed) return;

            ExamSession.ResetCurrentExam();
        }

        private void HandleReset() {
            try {
                ExamSession = new QuizSessionVM(User!.Id);
                ExamSessionRequest.CandidateId = User?.Id ?? Guid.Empty;
                ExamSessionRequest.ExamId = null;
                ExamSessionRequest.MaxResultCount = 20;
                ExamSessionRequest.IsRecursive = true;
                ExamSessionRequest.IsRandom = true;
            }
            finally {
            }
        }

        private void EndExamSession(MouseEventArgs evt) {
            if (ExamSession != null) {
                ExamSession.IsEnded = true;
                ExamSession.MoveToQuestion(0);
            }
        }

        private async Task EndAndSubmitExamSession(MouseEventArgs evt) {
            if ((ExamSession == null) || (ExamSession.TotalAnswers <= 0)) {
                //await JsRuntime.InvokeVoidAsync("alert", "Oops! There is no user session to save."); // Alert
                BlazoredModal.Show<Quiz.Blazor.Shared.DisplayMessage>("Oops! There is no user session to save");
                return;
            } else if (ExamSession.IsAlreadySubmitted) {
                //await JsRuntime.InvokeVoidAsync("alert", "Oops! Exam session has already been submitted."); // Alert
                BlazoredModal.Show<Quiz.Blazor.Shared.DisplayMessage>("Oops! Exam session has already been submitted");
                return;
            }

            var confirm = await BlazoredModal.Show<Quiz.Blazor.Shared.Confirm>(
                "Do you want to terminate and register your quiz session?",
                new ModalOptions { Position = ModalPosition.Middle, AnimationType = ModalAnimationType.FadeInOut }
                ).Result;
            if (!confirm.Confirmed) return;

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
                //await JsRuntime.InvokeVoidAsync("alert", "Oops! An error occurred trying to execute the requested operation: " + ex.Message);
                BlazoredModal.Show<Quiz.Blazor.Shared.DisplayMessage>("Oops! An error occurred trying to execute the requested operation: " + ex.Message);
            }
            finally {
            }

        }

    }

}