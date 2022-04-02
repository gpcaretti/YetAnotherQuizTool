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
            CandidateDto objHis = HttpContext.Session.GetObjectFromJson<CandidateDto>(QuizConstants.AuthUserKey);
            CandidateDto objCandidate = await _candidateAppService.FirstOrDefault(e => e.Id.Equals(objHis.Id));
            return View(objCandidate);
        }

        [BasicAuthentication]
        public async Task<IActionResult> Profile() {
            CandidateDto objHis = HttpContext.Session.GetObjectFromJson<CandidateDto>(QuizConstants.AuthUserKey);
            CandidateDto objCandidate = await _candidateAppService.FirstOrDefault(e => e.Id.Equals(objHis.Id));

            ProfileViewModel objModel = new ProfileViewModel() {
                Id = objCandidate.Id,
                Name = objCandidate.Name,
                Email = objCandidate.Email,
                Phone = objCandidate.Phone,
                ImgFile = objCandidate.ImgFile != null ? objCandidate.ImgFile : null
            };
            return View(objModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile([FromForm] ProfileViewModel profileVM) {
            int i = 0;
            string UploadFolder = null;
            string UniqueFileName = null;
            string UploadPath = null;
            if (ModelState.IsValid) {
                try {
                    if (profileVM.file != null) {
                        UploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles/Image");
                        UniqueFileName = Guid.NewGuid().ToString() + "_" + profileVM.file.FileName;
                        if (!System.IO.File.Exists(UploadFolder))
                            Directory.CreateDirectory(UploadFolder);
                        UploadPath = Path.Combine(UploadFolder, UniqueFileName);
                    }
                    CandidateDto candidateDto = await _candidateAppService.FindById(profileVM.Id);
                    candidateDto.Name = profileVM.Name;
                    candidateDto.Phone = profileVM.Phone;
                    candidateDto.Email = profileVM.Email;
                    candidateDto.ImgFile = !String.IsNullOrEmpty(UniqueFileName) ? UniqueFileName : candidateDto.ImgFile;
                    candidateDto.ModifiedBy = profileVM.Name;
                    profileVM.ImgFile = candidateDto.ImgFile;
                    i = await _candidateAppService.UpdateCandidate(candidateDto);
                    if (i > 0) {
                        if (profileVM.file != null) {
                            await profileVM.file.CopyToAsync(new FileStream(UploadPath, FileMode.Create));
                        }
                        ViewBag.Alert = AlertExtension.ShowAlert(Alerts.Success, "Profile updated successfully.");
                    } else
                        ViewBag.Alert = AlertExtension.ShowAlert(Alerts.Danger, "An error occurred.");
                } catch (Exception ex) {
                    ViewBag.Alert = AlertExtension.ShowAlert(Alerts.Danger, ex.Message);
                    if (ex.InnerException == null) throw;
                    throw new Exception(ex.Message, ex.InnerException);
                }
            } else
                ModelState.AddModelError("Error", "Unknown  Error.");

            return View(profileVM);
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
