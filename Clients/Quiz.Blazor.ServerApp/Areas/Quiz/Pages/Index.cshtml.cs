#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quiz.Application.Maintenance;

namespace Quiz.Blazor.ServerApp.Areas.Quiz.Pages {

    public class IndexModel : PageModel {

        private readonly ILogger<IndexModel> _logger;
        private readonly IDataExportAppService _exportService;

        public IndexModel(ILogger<IndexModel> logger, IDataExportAppService exportService) {
            _logger = logger;
            _exportService = exportService;
        }

        public string ErrorMessage { get; set; }

        public async Task OnGetAsync(string returnUrl = null) {
            if (!string.IsNullOrEmpty(ErrorMessage)) {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
        }



        public async Task<IActionResult> OnPostAsync(string returnUrl = null) {
            try {
                await _exportService.ExportToFilesDto(System.IO.Path.GetTempPath());
            }
            catch (Exception) {

                throw;
            }



            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}
