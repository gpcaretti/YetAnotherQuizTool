﻿using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using PatenteN.Quiz.Application.Exams;
using PatenteN.Quiz.Application.Web.Authentication;
using PatenteN.Quiz.Application.Web.Extensions;
using PatenteN.Quiz.Domain.Exams;
using PatenteN.Quiz.Domain.Users;

namespace PatenteN.Quiz.Application.Web.Controllers {
    [BasicAuthentication]
    public class ScoreController : Controller
    {
       private readonly ILogger<ScoreController> _logger;
       private readonly IResultAppService _result;
        private readonly IConverter _converter;

        public ScoreController(ILogger<ScoreController> logger, IResultAppService result, IConverter converter)
       {
            _logger = logger;
            _result = result;
            _converter = converter;
       }

       public async Task<IActionResult> Result()
       {
            try
            {
                Candidate _objCandidate = HttpContext.Session.GetObjectFromJson<Candidate>("AuthenticatedUser");

                IEnumerable<QuizAttempt> _obj = await _result.GetAttemptHistory(_objCandidate.Candidate_ID);
                Root objRoot = new Root(){
                    objCandidate= _objCandidate,
                    objAttempt = _obj.ToList() 
                };               
                return View(objRoot);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally 
            {
            }
        }

        [HttpPost]
        [Route("~/api/Report")]
        public async Task<IActionResult> Report(ReqReport argRpt)
        {
            try
            {
                IEnumerable<QuizReport> lst = await _result.ScoreReport(argRpt);
                return Ok(lst.ToList());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
            }
        }

        [HttpPost]
        [Route("~/api/CreatePDF")]
        public async Task<IActionResult> CreatePDF(ReqCertificate argPDFRpt)
        {
            ResPDF obj = null;
            try
            {                
                string html = await _result.GetCertificateString(argPDFRpt);
                string UploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles/Report");
                string UniqueFileName = argPDFRpt.CandidateID + "_Certificate.pdf";
                string UploadPath = Path.Combine(UploadFolder, UniqueFileName);
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10, Bottom = 10 },
                    Out = UploadPath
                };
                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" },
                };
                var htmlToPdfDocument = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings },
                };
                _converter.Convert(htmlToPdfDocument);
                obj = new ResPDF();
                obj.IsSuccess = true;
                obj.Path = "/UploadedFiles/Report/" + UniqueFileName;
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Path = null;
                throw new Exception(ex.Message, ex.InnerException);                 
            }
            finally
            {
            }
            return Json(obj);
        }
                
    }
}
