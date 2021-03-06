using MrCMS.Helpers;
using NHibernate;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;

namespace MrCMS.Web.Apps.Ecommerce.Services.Orders
{
    public class OrderNoteService : IOrderNoteService
    {
        private readonly ISession _session;

        public OrderNoteService(ISession session)
        {
            _session = session;
        }

        public void Save(OrderNote item)
        {
            _session.Transact(session => session.SaveOrUpdate(item));
        }

        public void Delete(OrderNote item)
        {
            _session.Transact(session => session.Delete(item));
        }

        public void AddOrderNoteAudit(string note, Order order)
        {
            var orderNote = new OrderNote
            {
                Note = note,
                ShowToClient = false,
                Order = order
            };
            order.OrderNotes.Add(orderNote);
            _session.Transact(session => session.SaveOrUpdate(orderNote));
        }
    }
}