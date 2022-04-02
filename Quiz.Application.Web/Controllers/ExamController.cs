using Microsoft.AspNetCore.Mvc;
using PatenteN.Quiz.Application.Exams;
using PatenteN.Quiz.Application.Web.Authentication;
using PatenteN.Quiz.Domain.Exams;

namespace PatenteN.Quiz.Application.Web.Controllers {

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

        [HttpGet]
        [Route("~/api/Exam/{ExamId?}")]
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

        [HttpGet]
        [Route("~/api/Questions/{ExamId?}")]
        public async Task<IActionResult> Questions(Guid examId) {
            try {
                var questionsDto = await _examAppService.GetQuestionsByExamId(examId);
                QnADto qna = await _questionAppService.GetQuestionListByExam(examId);
                return Ok(qna);
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

        [HttpPost]
        [Route("~/api/Score")]
        public async Task<IActionResult> Score(List<Option> objRequest) {
            int i = 0;
            bool IsCorrect = false;
            List<Result> objList = null;
            string sessionID = null;
            try {
                if (objRequest.Count > 0) {
                    sessionID = Guid.NewGuid().ToString() + "-" + DateTimeOffset.Now;
                    objList = new List<Result>();
                    foreach (var item in objRequest) {
                        if (item.AnswerId == item.SelectedOption)
                            IsCorrect = true;
                        else
                            IsCorrect = false;

                        Result obj = new Result() {
                            CandidateId = item.CandidateId,
                            ExamId = item.ExamId,
                            QuestionId = item.QuestionId,
                            AnswerId = item.AnswerId,
                            SelectedOptionId = item.SelectedOption,
                            IsCorrent = IsCorrect,
                            SessionId = sessionID,
                            CreatedBy = "SYSTEM",
                            CreatedOn = DateTimeOffset.Now
                        };
                        objList.Add(obj);
                    }
                    i = await _resultAppService.AddResults(objList);
                }

            } catch (Exception ex) {
                i = 0;
                if (ex.InnerException == null) throw;
                throw new Exception(ex.Message, ex.InnerException);
            } finally {
            }
            return Ok(i);
        }

    }
}
