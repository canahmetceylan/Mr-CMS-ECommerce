using System;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Services.Cart
{
    public interface IAssignPaymentInfo
    {
        CartModel Assign(CartModel cart, Guid userGuid);
    }
}