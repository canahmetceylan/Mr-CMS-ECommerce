using System.Web.Mvc;
using MrCMS.Website.Controllers;
using MrCMS.Web.Apps.Ecommerce.Services.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Website;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Controllers
{
    public class OrderNoteController : MrCMSAppAdminController<EcommerceApp>
    {
        private readonly IOrderNoteService _orderNoteService;
        private readonly IOrderService _orderService;

        public OrderNoteController(IOrderNoteService orderNoteService, IOrderService orderService)
        {
            _orderNoteService = orderNoteService;
            _orderService = orderService;
        }

        [HttpGet]
        public PartialViewResult Add(int orderID)
        {
            OrderNote orderNote = new OrderNote();
            orderNote.Order = _orderService.Get(orderID);
            orderNote.User = CurrentRequestData.CurrentUser;
            return PartialView(orderNote);
        }

        [ActionName("Add")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Add_POST(OrderNote orderNote)
        {
            if (orderNote.Order != null)
            {
                orderNote.Order.OrderNotes.Add(orderNote);
                _orderNoteService.Save(orderNote);
                return RedirectToAction("Edit", "Order", new { id = orderNote.Order.Id });
            }
            else
            {
                return RedirectToAction("Index", "Order");
            }
        }

        [HttpGet]
        public PartialViewResult Edit(OrderNote orderNote)
        {
            return PartialView(orderNote);
        }

        [ActionName("Edit")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Edit_POST(OrderNote orderNote)
        {
            if (orderNote.Order != null)
            {
                orderNote.User = CurrentRequestData.CurrentUser;
                _orderNoteService.Save(orderNote);
                return RedirectToAction("Edit", "Order", new { id = orderNote.Order.Id });
            }
            else
            {
                return RedirectToAction("Index", "Order");
            }
        }

        [HttpGet]
        public PartialViewResult Delete(OrderNote orderNote)
        {
            return PartialView(orderNote);
        }

        [ActionName("Delete")]
        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public RedirectToRouteResult Delete_POST(OrderNote orderNote)
        {
            if (orderNote.Order != null)
            {
                _orderNoteService.Delete(orderNote);
                return RedirectToAction("Edit", "Order", new { id = orderNote.Order.Id });
            }
            else
            {
                return RedirectToAction("Index", "Order");
            }
        }
    }
}