using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz.Application.Dtos;
using Quiz.Application.Exams;
using Quiz.Application.Sessions;

namespace Quiz.Blazor.Host.Server.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class QuizController : ControllerBase {

        private readonly ILogger<QuizController> _logger;
        private readonly IExamAppService _examAppService;
        private readonly IExamSessionAppService _examSessionAppService;

        public QuizController(ILogger<QuizController> logger, IExamAppService examAppService, IExamSessionAppService examSessionAppService) {
            _logger = logger;
            _examAppService = examAppService;
            _examSessionAppService = examSessionAppService;
        }

        [HttpGet]
        [AllowAnonymous]
        //[Authorize(Roles = $"{QuizConstants.Roles.Candidate},{QuizConstants.Roles.Manager},{QuizConstants.Roles.Admin}")]
        public async Task<ICollection<ExamDto>> Get() {
            var output = await _examAppService.GetAll(new PagedAndSortedResultRequestDto { MaxResultCount = 100, Sorting = nameof(ExamDto.Code) });

            return output;
        }
    }
}