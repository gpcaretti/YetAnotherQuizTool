using System.Diagnostics;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Quiz.Application.Dtos;
using Quiz.Application.Exams;
using Quiz.Application.Sessions;

namespace Quiz.Blazor.Shared {

    public partial class NewSessionBuilder {

        [Parameter]
        public PrepareExamSessionRequestDto? NewSessionRequest { get; set; }

        [Parameter]
        public EventCallback<PrepareExamSessionRequestDto> PrepareExamSessionSubmitted { get; set; }

        [CascadingParameter]
        public IModalService BlazoredModal { get; set; }

        [Inject] private IExamAppService _examAppService { get; set; }

        protected ICollection<ExamDto> AvailableExams = new List<ExamDto>(100);
        protected int WaitingCnt;

        public Stopwatch watch1;
        public Stopwatch watch2;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync() {
            WaitingCnt = 1;
            //StateHasChanged();
            try {
                // get available exams

                // FIXME: As SQLite has poor performance when using index, I prefer to execute the in-memery sorting
                //AvailableExams = await _examAppService.GetAll(new PagedAndSortedResultRequestDto { MaxResultCount = 100, Sorting = nameof(ExamDto.Code) });
                watch1 = System.Diagnostics.Stopwatch.StartNew();
                var dtos = await _examAppService.GetAll(new PagedAndSortedResultRequestDto { MaxResultCount = 100/*, Sorting = nameof(ExamDto.Code) */});
                watch1.Stop();
                (AvailableExams as List<ExamDto>)!.AddRange(dtos.OrderBy(e => e.Code));

                HandleReset();
                await base.OnInitializedAsync();
            }
            finally {
                if (WaitingCnt > 0) WaitingCnt--;
            }
        }

        /// <summary>
        ///		UI request for a new session
        /// </summary>
        private Task StartExamSessionClick(MouseEventArgs evt) {
            // if no exam selected, warn and return
            if ((NewSessionRequest?.ExamId ?? Guid.Empty) == Guid.Empty) {
                //await JsRuntime.InvokeVoidAsync("alert", "Please, select an exam"); // Alert
                BlazoredModal.Show<DisplayMessage>("Please, select an exam");
                return Task.CompletedTask;
            }
            else {
                //return PrepareExamSessionSubmitted?.InvokeAsync(NewSessionInput) ?? Task.CompletedTask;
                //return PrepareExamSessionSubmitted.InvokeAsync(NewSessionInput) ?? Task.CompletedTask;
                return PrepareExamSessionSubmitted.InvokeAsync(NewSessionRequest);
            }
        }

        private void HandleReset() {
            try {
                var examdto = AvailableExams.FirstOrDefault();
                NewSessionRequest.ExamId ??= AvailableExams.FirstOrDefault()?.Id ?? Guid.Empty;
                //NewSessionRequest.MaxResultCount = AvailableExams.FirstOrDefault()?.Duration ?? 20;    // FIXME: how to define the default num of questions per exam?
                NewSessionRequest.IsRecursive = true; // TODO: manage it soon or later
                                                      //NewSessionRequest.IsRandom = true;
            }
            finally {
            }
        }

        private void HandleValidSubmit() {
            // TODO
            //var modelJson = JsonSerializer.Serialize(NewQuizModel, new JsonSerializerOptions { WriteIndented = true });
            //JSRuntime.InvokeVoidAsync("alert", $"SUCCESS!! :-)\n\n{modelJson}");
        }
    }
}