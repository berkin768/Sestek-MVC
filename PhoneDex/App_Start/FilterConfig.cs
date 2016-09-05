using System.Web.Mvc;
using System.Web.Routing;
using PhoneDex.Core;

namespace PhoneDex
{
    public class IntranetAction : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(new LocationHelper().IsInSestekNetwork(filterContext.RequestContext.HttpContext.Request.UserHostAddress))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home", 
                    action = "Index"
                }));
            }

            base.OnActionExecuting(filterContext);
        }
    }
}