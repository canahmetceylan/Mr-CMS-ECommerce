using MrCMS.Web.Apps.Ecommerce.Entities;
using MrCMS.Web.Apps.Ecommerce.Entities.Discounts;

namespace MrCMS.Web.Apps.Ecommerce.Services.Discounts
{
    public interface IDiscountLimitationService
    {
        DiscountLimitation Get(int id);
    }
}
