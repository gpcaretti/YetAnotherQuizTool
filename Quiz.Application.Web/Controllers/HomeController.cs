using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using PatenteN.Quiz.Application.Users;
using PatenteN.Quiz.Application.Web.Authentication;
using PatenteN.Quiz.Application.Web.Enums;
using PatenteN.Quiz.Application.Web.Extensions;
using PatenteN.Quiz.Application.Web.Models;
using PatenteN.Quiz.Domain.Users;

namespace PatenteN.Quiz.Application.Web.Controllers {

    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly ICandidateAppService _candidateAppService;

        public HomeController(ILogger<HomeController> logger, ICandidateAppService candidateAppService) {
            _logger = logger;
            _candidateAppService = candidateAppService;
        }

        [BasicAuthentication]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index() {
            Candidate objHis = HttpContext.Session.GetObjectFromJson<Candidate>("AuthenticatedUser");
            CandidateDto objCandidate = await _candidateAppService.FirstOrDefault(e => e.Sl_No.Equals(objHis.Sl_No));
            return View(objCandidate);
        }

        [BasicAuthentication]
        public async Task<IActionResult> Profile() {
            Candidate objHis = HttpContext.Session.GetObjectFromJson<Candidate>("AuthenticatedUser");
            CandidateDto objCandidate = await _candidateAppService.FirstOrDefault(e => e.Sl_No.Equals(objHis.Sl_No));

            ProfileViewModel objModel = new ProfileViewModel() {
                Sl_No = objCandidate.Sl_No,
                Name = objCandidate.Name,
                Candidate_ID = objCandidate.Candidate_ID,
                Email = objCandidate.Email,
                Phone = objCandidate.Phone,
                ImgFile = objCandidate.ImgFile != null ? objCandidate.ImgFile : null
            };
            return View(objModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile([FromForm] ProfileViewModel argObj) {
            int i = 0;
            string UploadFolder = null;
            string UniqueFileName = null;
            string UploadPath = null;
            if (ModelState.IsValid) {
                try {
                    if (argObj.file != null) {
                        UploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles/Image");
                        UniqueFileName = Guid.NewGuid().ToString() + "_" + argObj.file.FileName;
                        UploadPath = Path.Combine(UploadFolder, UniqueFileName);
                    }
                    CandidateDto _objCandidate = await _candidateAppService.FindById(argObj.Sl_No);
                    _objCandidate.Name = argObj.Name;
                    _objCandidate.Candidate_ID = argObj.Candidate_ID;
                    _objCandidate.Phone = argObj.Phone;
                    _objCandidate.Email = argObj.Email;
                    if (UniqueFileName != null) { _objCandidate.ImgFile = UniqueFileName; } else { _objCandidate.ImgFile = _objCandidate.ImgFile; }
                    _objCandidate.ModifiedBy = argObj.Name;
                    argObj.ImgFile = _objCandidate.ImgFile;
                    i = await _candidateAppService.UpdateCandidate(_objCandidate);
                    if (i > 0) {
                        if (argObj.file != null) {
                            await argObj.file.CopyToAsync(new FileStream(UploadPath, FileMode.Create));
                        }
                        ViewBag.Alert = AlertExtension.ShowAlert(Alerts.Success, "Profile updated successfully.");
                    } else
                        ViewBag.Alert = AlertExtension.ShowAlert(Alerts.Danger, "An error occurred.");
                } catch (Exception ex) {
                    ViewBag.Alert = AlertExtension.ShowAlert(Alerts.Danger, ex.Message);
                    throw new Exception(ex.Message, ex.InnerException);
                }
            } else
                ModelState.AddModelError("Error", "Unknown  Error.");

            return View(argObj);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> SaveRecoredFile() {
            if (Request.Form.Files.Any()) {
                var file = Request.Form.Files["video-blob"];
                string UploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles/Video");
                string UniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName + ".webm";
                string UploadPath = Path.Combine(UploadFolder, UniqueFileName);
                await file.CopyToAsync(new FileStream(UploadPath, FileMode.Create));
            }
            return Json(HttpStatusCode.OK);
        }

    }
}
