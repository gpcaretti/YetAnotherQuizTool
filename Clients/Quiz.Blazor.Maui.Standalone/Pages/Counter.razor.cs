
using Quiz.Application.Sessions;

namespace Quiz.Blazor.Maui.Standalone.Pages {

    public partial class Counter {

        //[CascadingParameter]
        //public IModalService BlazoredModal { get; set; } = default!;
        private int currentCount = 0;

        //protected override Task OnInitializedAsync() {
        //    return base.OnInitializedAsync();
        //    //var v = DateTime.Now;
        //    //currentCount = (int)(v.Ticks % 1000);
        //}

        private void IncrementCount() {
            currentCount++;
        }

        protected Task MyHandler(PrepareExamSessionRequestDto input) {
            return Task.CompletedTask;
        }
    }
}