using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Quiz.Application.Exams;

namespace Quiz.Blazor.Host.Client.Pages {
    public partial class QuizViewer {

        protected ICollection<ExamDto>? AvailableExams;

        protected override async Task OnInitializedAsync() {
            try {
                AvailableExams = await Http.GetFromJsonAsync<ICollection<ExamDto>>("Quiz");
                if (_logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug($"Available exams: {AvailableExams?.Count ?? -1}");
            } catch (AccessTokenNotAvailableException exception) {
                exception.Redirect();
            }
        }
    }
}
