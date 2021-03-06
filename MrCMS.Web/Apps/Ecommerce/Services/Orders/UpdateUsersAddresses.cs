using System.Linq;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Users;
using MrCMS.Web.Apps.Ecommerce.Models;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class UpdateUsersAddresses : IOnOrderPlaced
    {
        private readonly ISession _session;

        public UpdateUsersAddresses(ISession session)
        {
            _session = session;
        }

        public void Execute(OrderPlacedArgs args)
        {
            _session.Transact(session =>
            {
                User currentUser = CurrentRequestData.CurrentUser;
                if (currentUser != null)
                {
                    var addresses = session.QueryOver<Address>().Where(address => address.User.Id == currentUser.Id).List();

                    var order = args.Order;
                    if (!addresses.Contains(order.BillingAddress, AddressComparison.Comparer))
                    {
                        var clone = order.BillingAddress.ToAddress(session, currentUser);
                        session.Save(clone);
                        addresses.Add(clone);
                    }
                    if (order.ShippingAddress != null && order.ShippingStatus != ShippingStatus.ShippingNotRequired &&
                        !addresses.Contains(order.ShippingAddress, AddressComparison.Comparer))
                    {
                        session.Save(order.ShippingAddress.ToAddress(session, currentUser));
                    }
                    if (string.IsNullOrEmpty(currentUser.FirstName) &&
                        string.IsNullOrEmpty(currentUser.LastName) &&
                        order.BillingAddress != null)
                    {
                        currentUser.FirstName = order.BillingAddress.FirstName;
                        currentUser.LastName = order.BillingAddress.LastName;
                        session.Save(currentUser);
                    }
                }
            });
        }
    }
}