using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatenteN.Quiz.Application.Users;
using PatenteN.Quiz.Application.Web.Extensions;
using PatenteN.Quiz.Application.Web.Models;
using PatenteN.Quiz.Domain.Users;

namespace PatenteN.Quiz.Application.Web.Controllers {
    public class AccountController : Controller {
        private const string AUTH_USER_KEY = "AuthenticatedUser";

        private readonly ILogger<AccountController> _logger;
        private readonly ICandidateAppService _candidateAppService;

        public AccountController(ILogger<AccountController> logger, ICandidateAppService candidateAppService) {
            _candidateAppService = candidateAppService;
            _logger = logger;
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
        public async Task<IActionResult> Register([FromForm] RegisterViewModel objCollection) {
            int i = 0;
            if (ModelState.IsValid) {
                try {
                    if (!await _candidateAppService.Any(e => e.Candidate_ID.Equals(objCollection.Candidate_ID)) == true) {
                        var _objcandidate = new CandidateDto() {
                            Name = objCollection.Name,
                            Email = objCollection.Email,
                            Phone = objCollection.Phone,
                            Candidate_ID = objCollection.Candidate_ID,
                            Roles = "User",
                            Password = objCollection.Password.EncodeBase64(),
                            CreatedBy = "SYSTEM",
                        };

                        i = await _candidateAppService.Create(_objcandidate);

                        if (i > 0)
                            return RedirectToAction("Login", "Account");
                        else
                            TempData["Message"] = "An error occurred.";
                    } else
                        TempData["Message"] = "A user already exists with that Candidate ID.";
                } catch (Exception ex) {
                    TempData["Message"] = ex.Message;
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
                string value = Convert.ToString(HttpContext.Session.GetString(AUTH_USER_KEY));

                if (string.IsNullOrEmpty(value))
                    return PartialView("_Login");
                else
                    return RedirectToAction("Index", "Home");
            } catch (Exception ex) {
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
                string value = Convert.ToString(HttpContext.Session.GetString(AUTH_USER_KEY));

                if (ModelState.IsValid) {
                    if (string.IsNullOrEmpty(value)) {
                        var candidate = await _candidateAppService.FirstOrDefault(x => x.Email.Equals(objCollection.Email) && x.Password.Equals(objCollection.Password.EncodeBase64()));
                        if (candidate != null) {
                            //+++candidate.Password = candidate.Password.EncodeBase64();
                            HttpContext.Session.SetObjectAsJson(AUTH_USER_KEY, candidate);
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
