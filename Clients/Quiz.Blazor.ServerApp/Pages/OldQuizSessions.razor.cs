using Microsoft.AspNetCore.Components.Authorization;
using Quiz.Application.Sessions;
using Quiz.Domain.Identity;

namespace Quiz.Blazor.ServerApp.Pages {

    public partial class OldQuizSessions /*: Microsoft.AspNetCore.Components.ComponentBase*/ {
        protected int WaitingCnt { get; set; }
        protected IDictionary<string, List<ExamSessionDto>>? UserSessions { get; set; }

        private ApplicationUser? _applicationUser;

        protected override async Task OnInitializedAsync() {
            await base.OnInitializedAsync();

            WaitingCnt = 1;
            try {
                // get the current user
                AuthenticationState authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                _applicationUser = await _userManager.GetUserAsync(authState.User);
                UserSessions = await GetUserSessionsGroupedByExam();
            }
            finally {
                if (WaitingCnt > 0) WaitingCnt--;
            }
        }

        private async Task<IDictionary<string, List<ExamSessionDto>>?> GetUserSessionsGroupedByExam() {
            var sessions = await _examSessionAppService.GetUserSessions(
                    new UserSessionsRequestDto {
                        CandidateId = Guid.Parse(_applicationUser!.Id),
                    });

            return (sessions?.Count ?? 0) > 0
                ? sessions!
                    .GroupBy(o => o.ExamId ?? Guid.Empty)
                    .ToDictionary(g => g.FirstOrDefault()?.ExamName ?? "n/a", g => g.ToList())
                : null;

        }
    }
}
