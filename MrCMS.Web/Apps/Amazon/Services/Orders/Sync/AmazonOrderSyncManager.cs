using System.Collections.Generic;
using System.Linq;
using MarketplaceWebServiceOrders.Model;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api;
using MrCMS.Web.Apps.Amazon.Services.Api.Orders;
using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Orders.Sync
{
    public class AmazonOrderSyncManager : IAmazonOrderSyncManager
    {
        private readonly IAmazonOrdersApiService _amazonOrdersApiService;
        private readonly IScheduleAmazonOrderSync _scheduleAmazonOrderSync;
        private readonly IShipAmazonOrderService _shipAmazonOrderService;
        private readonly IAmazonApiService _amazonApiService;

        public AmazonOrderSyncManager(IAmazonOrdersApiService amazonOrdersApiService,
            IShipAmazonOrderService shipAmazonOrderService, IAmazonApiService amazonApiService, 
            IScheduleAmazonOrderSync scheduleAmazonOrderSync)
        {
            _amazonOrdersApiService = amazonOrdersApiService;
            _shipAmazonOrderService = shipAmazonOrderService;
            _amazonApiService = amazonApiService;
            _scheduleAmazonOrderSync = scheduleAmazonOrderSync;
        }


        public GetUpdatedOrdersResult GetUpdatedInfoFromAmazon(GetUpdatedOrdersRequest updatedOrdersRequest)
        {
            if (_amazonApiService.IsLive(AmazonApiSection.Orders))
            {
                var orders = _amazonOrdersApiService.ListUpdatedOrders(updatedOrdersRequest)
                                           .Distinct(new StrictKeyEqualityComparer<Order, string>(order => order.AmazonOrderId));
                orders.Select(order => _scheduleAmazonOrderSync.ScheduleSync(order))
                                      .Where(amazonOrder => amazonOrder != null)
                                      .ToList();
                var ordersShipped = _shipAmazonOrderService.MarkOrdersAsShipped();
                return new GetUpdatedOrdersResult { OrdersShipped = ordersShipped };
            }
            return new GetUpdatedOrdersResult { ErrorMessage = "The service is not currently live" };
        }

        public GetUpdatedOrdersResult GetUpdatedInfoFromAmazonAdHoc(IEnumerable<string> amazonOrderIds)
        {
            if (_amazonApiService.IsLive(AmazonApiSection.Orders))
            {
                var orders = _amazonOrdersApiService.ListSpecificOrders(amazonOrderIds)
                     .Distinct(new StrictKeyEqualityComparer<Order, string>(order => order.AmazonOrderId)).ToList();
                if (orders.Any())
                {
                    orders.Select(order => _scheduleAmazonOrderSync.ScheduleSync(order))
                                      .Where(amazonOrder => amazonOrder != null)
                                      .ToList();
                    return new GetUpdatedOrdersResult {OrdersScheduledForSync = orders};
                }
                return new GetUpdatedOrdersResult { ErrorMessage = "We didn't found any Amazon Orders with provided Ids" };
            }
            return new GetUpdatedOrdersResult { ErrorMessage = "The service is not currently live" };
        }
    }
}