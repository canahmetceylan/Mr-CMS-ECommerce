using System.Linq;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public interface IScheduleAmazonOrderSync
    {
        AmazonOrder ScheduleSync(Order order);
    }
    public class ScheduleAmazonOrderSync : IScheduleAmazonOrderSync
    {
        private readonly IImportAmazonOrderService _importAmazonOrderService;
        private readonly IAmazonLogService _amazonLogService;
        private readonly IAmazonOrderService _amazonOrderService;
        private readonly AmazonOrderSyncDataService _amazonOrderSyncInfoService;

        public ScheduleAmazonOrderSync(IImportAmazonOrderService importAmazonOrderService, IAmazonLogService amazonLogService,
                                 IAmazonOrderService amazonOrderService, AmazonOrderSyncDataService amazonOrderSyncInfoService)
        {
            _importAmazonOrderService = importAmazonOrderService;
            _amazonLogService = amazonLogService;
            _amazonOrderService = amazonOrderService;
            _amazonOrderSyncInfoService = amazonOrderSyncInfoService;
        }

        public AmazonOrder ScheduleSync(Order order)
        {
            switch (order.OrderStatus)
            {
                case OrderStatusEnum.Unshipped:
                case OrderStatusEnum.PartiallyShipped:
                case OrderStatusEnum.Shipped:
                case OrderStatusEnum.Canceled:
                    {
                        if (!_importAmazonOrderService.IsCurrencyValid(order))
                        {
                            _amazonLogService.Add(AmazonLogType.Orders, AmazonLogStatus.Stage, null, null,
                                                  AmazonApiSection.Orders,
                                                  null, null, null, null,
                                                  string.Format(
                                                      "Amazon Order #{0} uses different currency than current MrCMS Site.",
                                                      order.AmazonOrderId));
                        }
                        else
                        {
                            var amazonOrder = _amazonOrderService.GetByAmazonOrderId(order.AmazonOrderId);
                            var amazonOrderData = _amazonOrderSyncInfoService.GetByAmazonOrderId(order.AmazonOrderId).ToList();

                            if (order.OrderStatus == OrderStatusEnum.Canceled) return amazonOrder;

                            var newAmazonOrderData = new AmazonOrderSyncData
                            {
                                OrderId = order.AmazonOrderId,
                                Operation = (amazonOrder == null
                                            ? (amazonOrderData.Any(x=>x.Operation==SyncAmazonOrderOperation.Add) ? SyncAmazonOrderOperation.Update : SyncAmazonOrderOperation.Add) : SyncAmazonOrderOperation.Update),
                                Status = SyncAmazonOrderStatus.Pending,
                                Data = AmazonAppHelper.SerializeToJson(order),
                                Site = CurrentRequestData.CurrentSite
                            };

                            if (amazonOrder != null)
                                newAmazonOrderData.AmazonOrder = amazonOrder;

                            _amazonOrderSyncInfoService.Add(newAmazonOrderData);

                            return amazonOrder;
                        }
                    }
                    break;
            }
            return null;
        }
    }
}