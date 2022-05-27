using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Quiz.Application.Exams;


namespace Quiz.Application.Blazor.Pages {
    public partial class QuizEdit : Microsoft.AspNetCore.Components.ComponentBase {

        [Parameter]
        public string SearchKey { get; set; }

        protected QuestionAndChoicesDto? Question { get; set; }

        protected IdentityUser<Guid> User;

        protected int WaitingCnt;

        private EditContext editContext;


        protected override async Task OnParametersSetAsync() {
            WaitingCnt = 1;
            try {
                HandleReset();

                // get the current user
                AuthenticationState authState = await GetAuthenticationStateAsync.GetAuthenticationStateAsync();
                User = await UserManager.GetUserAsync(authState.User);

                // get the question
                Question = await _questionAppService.FindBySearchKey(SearchKey);

                //editContext.OnValidationRequested += HandleValidationRequested;
            } finally {
                if (WaitingCnt > 0) WaitingCnt--;
            }
        }

        // Locally register correct answer for the current question
        private void RegisterUserAnswer(Guid choiceId) {
            //var answer = ExamSession?.Answers?.FirstOrDefault(ans => ans.QuestionId == questionId);
            //if (answer != null) {
            //    answer.UserChoiceId = choiceId;
            //}
        }

        private void HandleReset() {
            try {
                //// TODO
                //ExamSession = new QuizSession() {
                //    CandidateId = User?.Id,
                //};

                //NewQuizModel = new PrepareExamSessionRequestDto {
                //    CandidateId = User?.Id,
                //    IsRandom = true,
                //    MaxResultCount = 20,    // FIXME: how to define the default num of questions per exam?
                //};
                if (Question != null) editContext = new EditContext(Question);
            } finally {
            }
        }

        private void HandleValidSubmit() {
            // TODO
            //var modelJson = JsonSerializer.Serialize(NewQuizModel, new JsonSerializerOptions { WriteIndented = true });
            //JSRuntime.InvokeVoidAsync("alert", $"SUCCESS!! :-)\n\n{modelJson}");
        }

    }
}
