using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quiz.Application.Guids;
using Quiz.Application.Users;
using Quiz.Application.Web.Extensions;
using Quiz.Application.Web.Models;
using Quiz.Domain.Users;

namespace Quiz.Application.Web.Controllers {
    public class AccountController : Controller {
        private readonly ICandidateAppService _candidateAppService;
        private readonly IGuidGenerator GuidGenerator;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger,
            ICandidateAppService candidateAppService,
            IGuidGenerator guidGenerator) {
            _logger = logger;
            _candidateAppService = candidateAppService;
            GuidGenerator = guidGenerator;
        }

        // GET: AccountController
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() {
            if (TempData["Message"] != null) {
                TempData["Message"] = null;
            }
            return PartialView("_Register");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel registerVM) {
            int i = 0;
            if (ModelState.IsValid) {
                try {
                    registerVM.Email = registerVM.Email?.ToLowerInvariant().Trim();
                    if (!await _candidateAppService.Any(e => e.Email == registerVM.Email)) {
                        var dto = new CandidateDto() {
                            Id = GuidGenerator.Create(),
                            Name = registerVM.Name,
                            Email = registerVM.Email,
                            Phone = registerVM.Phone,
                            Roles = "User",
                            Password = registerVM.Password.EncodeBase64(),
                            CreatedBy = "SYSTEM",
                        };

                        i = await _candidateAppService.Create(dto);

                        if (i > 0)
                            return RedirectToAction("Login", "Account");
                        else
                            TempData["Message"] = "An error occurred.";
                    } else
                        TempData["Message"] = "A user already exists with that email address.";
                } catch (Exception ex) {
                    TempData["Message"] = ex.Message;
                    if (ex.InnerException == null) throw;
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }
            return PartialView("_Register");
        }

        // GET: AccountController
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login() {
            try {
                string _Action = string.Empty;
                string _Controller = string.Empty;
                string value = Convert.ToString(HttpContext.Session.GetString(QuizConstants.AuthUserKey));

                if (string.IsNullOrEmpty(value))
                    return PartialView("_Login");
                else
                    return RedirectToAction("Index", "Home");
            } catch (Exception ex) {
                if (ex.InnerException == null) throw;
                throw new Exception(ex.Message, ex.InnerException);
            } finally {
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginViewModel objCollection) {
            try {
                string _Action = string.Empty;
                string _Controller = string.Empty;
                string value = Convert.ToString(HttpContext.Session.GetString(QuizConstants.AuthUserKey));

                if (ModelState.IsValid) {
                    if (string.IsNullOrEmpty(value)) {
                        objCollection.Email = objCollection.Email.ToLowerInvariant().Trim();
                        var candidateDto = await _candidateAppService.FirstOrDefault(x => x.Email.Equals(objCollection.Email) && x.Password.Equals(objCollection.Password.EncodeBase64()));
                        if (candidateDto != null) {
                            //+++candidate.Password = candidate.Password.EncodeBase64();
                            HttpContext.Session.SetObjectAsJson(QuizConstants.AuthUserKey, candidateDto);
                            _Controller = "Home";
                            _Action = "Index";
                        } else {
                            TempData["Message"] = "Invalid User.";
                            _Controller = "Account";
                            _Action = "Login";
                        }
                    } else {
                        _Controller = "Account";
                        _Action = "Login";
                    }
                }
                return RedirectToAction(_Action, _Controller, ViewBag.Alert);
            } catch (Exception ex) {
                if (ex.InnerException == null) throw;
                throw new Exception(ex.Message, ex.InnerException);
            } finally {
            }
        }

        [HttpGet]
        public IActionResult Logout() {
            try {
                foreach (var cookie in Request.Cookies.Keys) {
                    Response.Cookies.Delete(cookie);
                }
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            } catch (Exception ex) {
                if (ex.InnerException == null) throw;
                throw new Exception(ex.Message, ex.InnerException);
            } finally {
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Reset() {
            if (TempData["Message"] != null) {
                TempData["Message"] = null;
            }
            return PartialView("_Reset");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reset([FromForm] ResetViewModel objCollection) {
            if (ModelState.IsValid) {
                int i = 0;
                var candidate = await _candidateAppService.FirstOrDefault(e => e.Email.Equals(objCollection.Email));
                if (candidate != null) {
                    candidate.ModifiedBy = objCollection.Email;

                    i = await _candidateAppService.UpdateCandidate(candidate);

                    if (i > 0)
                        return RedirectToAction("Login", "Account");
                    else
                        TempData["Message"] = "An error occurred.";
                } else
                    TempData["Message"] = "Invalid Email.";
            }
            return PartialView("_Reset");
        }

    }
}
