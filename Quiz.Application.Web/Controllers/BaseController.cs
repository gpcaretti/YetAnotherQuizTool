using Microsoft.AspNetCore.Mvc;
using Quiz.Application.Guids;
using Quiz.Application.Users;
using Quiz.Application.Web.Extensions;

namespace Quiz.Application.Web.Controllers {

    public abstract class BaseController : Controller {

        protected readonly ILogger _logger;
        protected readonly IGuidGenerator GuidGenerator;
        protected readonly ICandidateAppService _candidateAppService;

        public BaseController(ILogger logger, IGuidGenerator guidGenerator, ICandidateAppService candidateAppService) {
            _logger = logger;
            _candidateAppService = candidateAppService;
            GuidGenerator = guidGenerator;
        }

        /// <summary>
        ///     Can be null
        /// </summary>
        /// <returns>Can be null</returns>
        protected async Task<CandidateDto> GetCurrentLoggedInUser() {
            CandidateDto objHis = HttpContext.Session.GetObjectFromJson<CandidateDto>(QuizConstants.AuthUserKey);
            return (objHis != null) ? await _candidateAppService.FirstOrDefault(e => e.Id.Equals(objHis.Id)) : null;
        }

    }
}
