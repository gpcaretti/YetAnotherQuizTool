using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Quiz.Application.Web.Authentication {
    public class BasicAuthentication : ActionFilterAttribute {
        public override void OnActionExecuting(ActionExecutingContext context) {
            string value = Convert.ToString(context.HttpContext.Session.GetString(QuizConstants.AuthUserKey));

            if (ReferenceEquals(value, null)) {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Account" }, { "Action", "Login" } });
            }
        }
    }
}