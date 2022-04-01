using System.Net;
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
                IEnumerable<ExamDto> lst = await _examAppService.Search();
                return Ok(lst.ToList());
            } catch (Exception ex) {
                throw new Exception(ex.Message, ex.InnerException);
            } finally {
            }
        }

        [HttpGet]
        [Route("~/api/Exam/{ExamID?}")]
        public async Task<IActionResult> Exam(int ExamID) {
            try {
                ExamDto dto = await _examAppService.FindById(ExamID);
                return Ok(dto);
            } catch (Exception ex) {
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
        [Route("~/api/Questions/{ExamID?}")]
        public async Task<IActionResult> Questions(int ExamID) {
            try {
                QnADto _obj = await _questionAppService.GetQuestionList(ExamID);
                return Ok(_obj);
            } catch (Exception ex) {
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
            string _SessionID = null;
            try {
                if (objRequest.Count > 0) {
                    _SessionID = Guid.NewGuid().ToString() + "-" + DateTime.Now;
                    objList = new List<Result>();
                    foreach (var item in objRequest) {
                        if (item.AnswerID == item.SelectedOption)
                            IsCorrect = true;
                        else
                            IsCorrect = false;

                        Result obj = new Result() {
                            CandidateID = item.CandidateID,
                            ExamID = item.ExamID,
                            QuestionID = item.QuestionID,
                            AnswerID = item.AnswerID,
                            SelectedOptionID = item.SelectedOption,
                            IsCorrent = IsCorrect,
                            SessionID = _SessionID,
                            CreatedBy = "SYSTEM",
                            CreatedOn = DateTime.Now
                        };
                        objList.Add(obj);
                    }
                    i = await _resultAppService.AddResults(objList);
                }

            } catch (Exception ex) {
                i = 0;
                throw new Exception(ex.Message, ex.InnerException);
            } finally {
            }
            return Ok(i);
        }

    }
}
