using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Settings;
using MrCMS.Web.Apps.Amazon.Services.Orders.Sync;
using MrCMS.Web.Apps.Amazon.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Services.Orders
{
    public interface IAmazonOrderSyncService
    {
        void Sync();
        GetUpdatedOrdersResult SyncSpecificOrders(string rawOrderIds);
    }

    public class AmazonOrderSyncService : IAmazonOrderSyncService
    {
        private readonly IAmazonOrderSyncManager _amazonOrderSyncManager;
        private readonly IConfigurationProvider _configurationProvider;

        public AmazonOrderSyncService(IAmazonOrderSyncManager amazonOrderSyncManager, IConfigurationProvider configurationProvider)
        {
            _amazonOrderSyncManager = amazonOrderSyncManager;
            _configurationProvider = configurationProvider;
        }

        public void Sync()
        {
            var amazonSyncSettings = _configurationProvider.GetSiteSettings<AmazonSyncSettings>();
            var lastRun = amazonSyncSettings.LastRun;

            DateTime @from;
            var now = CurrentRequestData.Now;
            var to = now.AddMinutes(-30);
            if (lastRun.HasValue)
            {
                if (lastRun > to)
                    return;
                @from = lastRun.Value;
            }
            else
                @from = now.AddMonths(-2);
            var updatedOrdersRequest = new GetUpdatedOrdersRequest
            {
                LastUpdatedAfter = @from,
                LastUpdatedBefore = to
            };
            amazonSyncSettings.LastRun = to;
            _configurationProvider.SaveSettings(amazonSyncSettings);
            _amazonOrderSyncManager.GetUpdatedInfoFromAmazon(updatedOrdersRequest);
        }

        public GetUpdatedOrdersResult SyncSpecificOrders(string rawOrderIds)
        {
            var orderIds = GetOrderIds(rawOrderIds);
            if (orderIds.Any())
            {
                return orderIds.Count <= 50 ? _amazonOrderSyncManager.GetUpdatedInfoFromAmazonAdHoc(orderIds) 
                    : new GetUpdatedOrdersResult() { ErrorMessage = "You can only sync up to 50 orders in one batch." };
            }
            return new GetUpdatedOrdersResult() { ErrorMessage = "Please provide at least one valid Amazon Order Id." };
        }

        private static List<string> GetOrderIds(string rawOrderIds)
        {
            var orderIds = new List<string>();
            try
            {
                var ids = rawOrderIds.Trim().Replace("\n", "").Replace("\r", "").Split(',');
                orderIds.AddRange(ids.Where(id => !String.IsNullOrWhiteSpace(id)));
            }
            catch (Exception ex)
            {
                CurrentRequestData.ErrorSignal.Raise(ex);
            }
            return orderIds;
        }
    }
}