using Microsoft.AspNetCore.Mvc;
using Quiz.Application.Exams;
using Quiz.Application.Guids;
using Quiz.Application.Sessions;
using Quiz.Application.Users;
using Quiz.Application.Web.Authentication;

namespace Quiz.Application.Web.Controllers {

    [BasicAuthentication]
    public class ExamController : BaseController {
        private readonly IExamAppService _examAppService;
        private readonly IQuestionAppService _questionAppService;

        public ExamController(
            ILogger<ExamController> logger,
            IGuidGenerator guidGenerator,
            ICandidateAppService candidateAppService,
            IExamAppService examAppService,
            IQuestionAppService questionAppService)
            : base(logger, guidGenerator, candidateAppService) {
            _examAppService = examAppService;
            _questionAppService = questionAppService;
        }

        [HttpGet]
        [Route("~/api/Exams")]
        public async Task<IActionResult> Exams() {
            try {
                IEnumerable<ExamDto> lst = await _examAppService.Search(null, $"{nameof(ExamDto.Code)}");
                return Ok(lst.ToList());
            } catch (Exception ex) {
                _logger.LogError(ex, nameof(Exams));
                if (ex.InnerException == null) throw;
                throw new Exception(ex.Message, ex.InnerException);
            } finally {
            }
        }

        /// <summary>
        ///     Get info about an exam
        /// </summary>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        [Route("~/api/Exam/{examId?}")]
        public async Task<IActionResult> Exam(Guid examId) {
            try {
                ExamDto dto = await _examAppService.FindById(examId);
                if (dto == null) throw new Exception($"Exam not found (id: ${examId})");
                return Ok(dto);
            } catch (Exception ex) {
                _logger.LogError(ex, nameof(Exam));
                if (ex.InnerException == null) throw;
                throw new Exception(ex.Message, ex.InnerException);
                //Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //return Json(new {
                //    Result = false,
                //    Error = ex.Message,
                //    Exception = ex.InnerException, 
                //});
            } finally {
            }
        }

        /// <summary>
        ///     Prepare an exam session based on the passed exam Id
        /// </summary>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [Route("~/api/PrepareExamSession/{input?}")]
        public async Task<IActionResult> PrepareExamSession(PrepareExamSessionRequestDto input) {
            try {
                if (!ModelState.IsValid) return Ok(new { IsSuccess = false, Message = "Invalid posted data" });

                CandidateDto candidate = await GetCurrentLoggedInUser();
                PrepareExamSessionResponseDto examSession = await _examAppService.PrepareExamSession(input, candidate);

                return Ok(new {
                    IsSuccess = true,
                    Message = "Sesssion saved!",
                    data = examSession
                });
            } catch (Exception ex) {
                _logger.LogError(ex, nameof(PrepareExamSession));
                if (ex.InnerException == null) throw;
                throw new Exception(ex.Message, ex.InnerException);
                //Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //return Json(new {
                //    Result = false,
                //    Error = ex.Message,
                //    Exception = ex.InnerException, 
                //});
            } finally {
            }
        }

        /// <summary>
        ///     Save the passed test results
        /// </summary>
        /// <param name="input"></param>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [Route("~/api/Score")]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Score(ExamSessionResultsRequestDto input) {
            try {
                if (!ModelState.IsValid) return Ok(new { IsSuccess = false, Message = "Invalid posted data (too much large?)" });

                CandidateDto candidate = await GetCurrentLoggedInUser() ?? await _candidateAppService.FindById(input.CandidateId);
                Guid savedSessionId = await _examAppService.SaveUserSession(input, candidate);

                return Ok(new {
                    IsSuccess = true,
                    Message = "Sesssion saved!",
                    data = savedSessionId
                });
            } catch (Exception ex) {
                _logger.LogError(ex, nameof(Score));
                if (ex.InnerException == null) throw;
                throw new Exception(ex.Message, ex.InnerException);
                //Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //return Json(new {
                //    Result = false,
                //    Error = ex.Message,
                //    Exception = ex.InnerException, 
                //});
            } finally {
            }

        }

    }
}
