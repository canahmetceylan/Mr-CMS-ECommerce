using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Ecommerce.Filters;
using MrCMS.Web.Apps.Ecommerce.ModelBinders;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Models.UserAccount;
using MrCMS.Web.Apps.Ecommerce.Pages;
using MrCMS.Web.Apps.Ecommerce.Services.UserAccount;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Ecommerce.Controllers
{
    public class UserAccountRewardPointsController : MrCMSAppUIController<EcommerceApp>
    {
        private readonly IGetUserRewardPointsStatement _getUserRewardPointsStatement;

        public UserAccountRewardPointsController(IGetUserRewardPointsStatement getUserRewardPointsStatement)
        {
            _getUserRewardPointsStatement = getUserRewardPointsStatement;
        }

        [MustBeLoggedIn]
        public ActionResult Show(UserAccountRewardPoints page, [IoCModelBinder(typeof(UserAccountRewardPointsModelBinder))] UserAccountRewardPointsSearchModel model)
        {
            User user = CurrentRequestData.CurrentUser;
            ViewData["rewards"] = _getUserRewardPointsStatement.Get(user, model.Page);
            ViewData["balance-details"] = _getUserRewardPointsStatement.GetDetails(user);

            return View(page);
        }
    }
}