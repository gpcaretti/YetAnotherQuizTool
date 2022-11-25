using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Quiz.Application.Sessions;
using Quiz.Application.Users;

namespace Quiz.Blazor.Shared.Pages {

    public partial class OverallSessionStatsButtons {

        // Demonstrates how a parent component can supply parameters
        [Parameter]
        public Guid? RootExamId { get; set; } = default!;
        [Parameter]
        public CandidateDto? User { get; set; }

        [Parameter]
        public bool NeedToRefresh {
            get {
                return _needToRefresh;
            }

            set {
                if (value != _needToRefresh) {
                    _needToRefresh = value;
                    NeedToRefreshChanged?.InvokeAsync(value);
                }
            }
        }

        [Parameter]
        public EventCallback<bool>? NeedToRefreshChanged { get; set; }

        private bool _needToRefresh;
        protected int WaitingCnt;

        [Inject]
        private IExamSessionAppService _examSessionAppService { get; set; }

        [Inject]
        private IJSRuntime JsRuntime { get; set; }

        [Inject]
        private ILogger<OverallSessionStatsButtons> _logger { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name = "rootExamId"></param>
        /// <returns></returns>
        protected async Task DeleteClosedSessions(Guid? rootExamId) {
            bool confirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Do you want to delete old exam sessions?"); // Confirm
            if (!confirmed)
                return;
            WaitingCnt++;
            try {
                NeedToRefresh = false;
                var total = await _examSessionAppService.DeleteUserSessions(new UserSessionsRequestDto {//CandidateId = (User != null) ? Guid.Parse(User!.Id) : null,
                    CandidateId = User?.Id,
                    ExamId = rootExamId,
                    MaxDeep = 10,
                });
                NeedToRefresh = (total > 0);
                _ = JsRuntime.InvokeVoidAsync("alert", $"{total} user's sessions deleted"); // Alert
            }
            catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
                _ = JsRuntime.InvokeVoidAsync("alert", "Oops! An error occurred trying to execute the requested operation: " + ex.Message);
            }
            finally {
                if (WaitingCnt > 0)
                    WaitingCnt--;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name = "rootExamId"></param>
        /// <returns></returns>
        protected async Task DeleteOldNotes(Guid? rootExamId) {
            bool confirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Do you want to delete old notes and errors?"); // Confirm
            if (!confirmed)
                return;
            WaitingCnt++;
            try {
                NeedToRefresh = false;
                var total = await _examSessionAppService.DeleteCandidateNotes(new UserSessionsRequestDto { CandidateId = User?.Id, ExamId = rootExamId, MaxDeep = 10, });
                _ = JsRuntime.InvokeVoidAsync("alert", $"{total} user's notes and error deleted"); // Alert
                NeedToRefresh = (total > 0);
            }
            catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
                _ = JsRuntime.InvokeVoidAsync("alert", "Oops! An error occurred trying to execute the requested operation: " + ex.Message);
            }
            finally {
                if (WaitingCnt > 0)
                    WaitingCnt--;
            }
        }
    }
}