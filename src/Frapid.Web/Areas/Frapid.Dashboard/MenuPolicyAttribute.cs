using System.Linq;
using System.Web.Mvc;
using Frapid.ApplicationState.Cache;
using Frapid.Dashboard.DAL;
using Frapid.Framework.Extensions;
using Frapid.Configuration;

namespace Frapid.Dashboard
{
    public class MenuPolicyAttribute : ActionFilterAttribute
    {
        public string OverridePath { get; set; }
        public bool StatusResponse { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string path = this.OverridePath.Or(filterContext.HttpContext.Request.FilePath);

            var my = AppUsers.GetCurrentAsync().Result;
            int userId = my.UserId;
            int officeId = my.OfficeId;
            string culture = my.Culture;

            string tenant = TenantConvention.GetTenant();

            var policy = Menu.GetAsync(tenant, userId, officeId, culture).Result;

            if (!policy.Any(x => x.Url.Equals(path)))
            {
                if (this.StatusResponse)
                {
                    filterContext.Result = new HttpUnauthorizedResult("Access is denied.");
                }
                else
                {
                    filterContext.Result = new RedirectResult("/account/sign-in");
                }
            }
        }
    }
}