using System.Web;
using System.Web.Mvc;
using PhoneDex.Core;

namespace PhoneDex.LDAP
{
    public class AuthorizeIndex : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return new LocationHelper().IsInSestekNetwork(httpContext.Request.UserHostAddress);
        }
    }
}