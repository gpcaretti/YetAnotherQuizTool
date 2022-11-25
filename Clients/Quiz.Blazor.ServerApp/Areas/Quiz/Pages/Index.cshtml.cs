#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Quiz.Blazor.ServerApp.Areas.Quiz.Pages {

    public class IndexModel : PageModel {

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger) {
            _logger = logger;
        }

        public string ErrorMessage { get; set; }

        public async Task OnGetAsync(string returnUrl = null) {
            if (!string.IsNullOrEmpty(ErrorMessage)) {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
        }



        public async Task<IActionResult> OnPostAsync(string returnUrl = null) {
            try {
                throw new NotImplementedException();
            }
            catch (Exception) {

                throw;
            }



            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}
