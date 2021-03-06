using MrCMS.Messages;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.MessageTemplates
{
    public class GetDefaultProductBackInStockMessageTemplate : GetDefaultTemplate<ProductBackInStockMessageTemplate>
    {
        public override ProductBackInStockMessageTemplate Get()
        {
            return new ProductBackInStockMessageTemplate
            {
                FromName = CurrentRequestData.CurrentSite.Name,
                ToAddress = "{Email}",
                ToName = "Customer",
                Bcc = string.Empty,
                Cc = string.Empty,
                Subject = string.Format("{0} - Product Back In Stock", CurrentRequestData.CurrentSite.Name),
                Body = "<p>The product {Name} is back in stock.</p><p>{ProductUrl}</p>",
                IsHtml = true
            };
        }
    }
}