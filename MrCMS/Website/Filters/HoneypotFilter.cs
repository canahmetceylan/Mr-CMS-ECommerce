using System.Web.Mvc;
using MrCMS.Settings;

namespace MrCMS.Website.Filters
{
    public class HoneypotFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!CurrentRequestData.DatabaseIsInstalled)
                return;
            if (!string.IsNullOrWhiteSpace(
                filterContext.HttpContext.Request[MrCMSApplication.Get<SiteSettings>().HoneypotFieldName]))
            {
                filterContext.Result = new EmptyResult();
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}