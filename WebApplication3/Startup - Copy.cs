using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Linq;
using System.Web.Mvc;
using WebApplication3.Models;


namespace WebApplication3
{
    public class AuthorizeForAdmin : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    filterContext.Result = new RedirectResult("~/Account/Login");
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/Home/Index");
                }
            }
        }
    }


    public class AuthorizeForAdminUser : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.IsInRole("Admin") || filterContext.HttpContext.User.IsInRole("User")) 
                  
            {
                base.OnAuthorization(filterContext);
            }
                else
                {
                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    filterContext.Result = new RedirectResult("~/Account/Login");
                }

            }
        }

    }

    public class AuthorizeForSpecificUserAndAdmin : AuthorizeAttribute
    {
       // public override void OnAuthorization()
    }

    public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
        {
            return controllerContext.RequestContext.HttpContext.Request.IsAjaxRequest();
        }
    }

}
