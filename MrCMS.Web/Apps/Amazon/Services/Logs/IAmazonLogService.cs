using System;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Entities.Logs;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;

namespace MrCMS.Web.Apps.Amazon.Services.Logs
{
    public interface IAmazonLogService
    {
        AmazonLog Add(AmazonLogType type, AmazonLogStatus status, Exception elmahError,
                      MarketplaceWebService.Model.Error amazonError,
                      AmazonApiSection? apiSection, string apiOperation, AmazonOrder amazonOrder,
                      AmazonListing amazonListing, AmazonListingGroup amazonListingGroup,
                      string message = "", string details = "");

        IPagedList<AmazonLog> GetEntriesPaged(int pageNum, AmazonLogType? type = null,
                                                AmazonLogStatus? status = null, int pageSize = 10);
        void DeleteAllLogs();
    }
}