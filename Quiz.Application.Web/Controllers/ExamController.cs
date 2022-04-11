using Microsoft.AspNetCore.Mvc;
using Quiz.Application.Exams;
using Quiz.Application.Web.Authentication;

namespace Quiz.Application.Web.Controllers {

    [BasicAuthentication]
    public class ExamController : Controller {
        private readonly ILogger<ExamController> _logger;
        private readonly IExamAppService _examAppService;
        private readonly IQuestionAppService _questionAppService;
        private readonly IResultAppService _resultAppService;
        public ExamController(
            ILogger<ExamController> logger,
            IExamAppService examAppService,
            IQuestionAppService questionAppService,
            IResultAppService resultAppService) {
            _logger = logger;
            _examAppService = examAppService;
            _questionAppService = questionAppService;
            _resultAppService = resultAppService;
        }

        [HttpGet]
        [Route("~/api/Exams")]
        public async Task<IActionResult> Exams() {
            try {
                IEnumerable<ExamDto> lst = await _examAppService.Search(null, $"{nameof(ExamDto.Code)}");
                return Ok(lst.ToList());
            } catch (Exception ex) {
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
        [HttpGet]
        [Route("~/api/PrepareExamSession/{input?}")]
        public async Task<IActionResult> PrepareExamSession(PrepareExamSessionRequestDto input) {
            try {
                PrepareExamSessionResponseDto examSession = await _questionAppService.PrepareExamSession(input);
                return Ok(examSession);
            } catch (Exception ex) {
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
        /// <param name="qnaResults"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        //[HttpPost]
        //[Route("~/api/Score")]
        //public async Task<IActionResult> Score(QnAResultsDto qnaResults) {
        //    bool isCorrect = false;
        //    List<Result> objList = null;
        //    string sessionId = null;
        //    int nSaved = 0;
        //    try {
        //        if (qnaResults.Answers.Count > 0) {ite.js
        //            sessionId = Guid.NewGuid().ToString() + "-" + DateTimeOffset.Now;
        //            objList = new List<Result>();
        //            foreach (var item in qnaResults.Answers) {
        //                Result obj = new Result() {
        //                    CandidateId = qnaResults.CandidateId,
        //                    ExamId = item.ExamId,
        //                    QuestionId = item.QuestionId,
        //                    //FIXME SelectedChoiceId = item.ChoiceId,
        //                    IsCorrent = isCorrect,
        //                    SessionId = sessionId,
        //                    CreatedBy = "SYSTEM",
        //                    CreatedOn = DateTimeOffset.Now
        //                };
        //                objList.Add(obj);
        //            }
        //            nSaved = await _resultAppService.AddResults(objList);
        //            return Ok(nSaved);
        //        }
        //    } catch (Exception ex) {
        //        if (ex.InnerException == null) throw;
        //        throw new Exception(ex.Message, ex.InnerException);
        //    } finally {
        //    }
        //    return Ok(nSaved);
        //}

    }
}
