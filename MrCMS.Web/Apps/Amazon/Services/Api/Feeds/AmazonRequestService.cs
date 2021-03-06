using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Helpers;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Web.Apps.Amazon.Services.Api.Products;
using MrCMS.Web.Apps.Amazon.Services.Listings;
using MrCMS.Web.Apps.Amazon.Services.Logs;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Amazon.Services.Api.Feeds
{
    public interface IAmazonListingRequestService
    {
        List<string> SubmitMainFeeds(AmazonSyncModel model, List<FileStream> feeds);
        string SubmitCloseRequest(AmazonSyncModel model, FileStream productFeedContent);

        void CheckIfDeleteRequestWasProcessed(AmazonSyncModel model, AmazonListing amazonListing, string submissionId);
        void CheckIfRequestsWereProcessed(AmazonSyncModel model, AmazonListingGroup item, List<string> submissionIds);
        void CheckIfRequestWasProcessed(AmazonSyncModel model, AmazonListing amazonListing, List<string> submissionIds);

    }

    public class AmazonRequestService : IAmazonListingRequestService
    {
        private readonly IAmazonListingService _amazonListingService;
        private readonly IAmazonFeedsApiService _amazonFeedsApiService;
        private readonly IAmazonProductsApiService _amazonProductsApiService;
        private readonly IAmazonLogService _amazonLogService;

        public AmazonRequestService(IAmazonListingService amazonListingService, IAmazonLogService amazonLogService,
            IAmazonFeedsApiService amazonFeedsApiService,
            IAmazonProductsApiService amazonProductsApiService)
        {
            _amazonListingService = amazonListingService;
            _amazonLogService = amazonLogService;
            _amazonFeedsApiService = amazonFeedsApiService;
            _amazonProductsApiService = amazonProductsApiService;
        }

        public List<string> SubmitMainFeeds(AmazonSyncModel model, List<FileStream> feeds)
        {
            var submissionIds = new List<string>();
            var uploadSuccess = false;
            var retryCount = 0;
            while (!uploadSuccess)
            {
                retryCount++;
                if (retryCount == 3)
                {
                    AmazonProgressBarHelper.Update(model.Task, "Error",
                                                  "Request timed out. Please check logs for potential errors and try again later.", 100,
                                                  100);
                    break;
                }

                try
                {
                    if (feeds.Count >= 1)
                        SubmitProductFeed(model, feeds.First(), ref submissionIds);

                    if (feeds.Count >= 2)
                        SubmitProductPriceFeed(model, feeds[1], ref submissionIds);

                    if (feeds.Count >= 3)
                        SubmitProductInventoryFeed(model, feeds[2], ref submissionIds);

                    uploadSuccess = true;
                }
                catch (Exception ex)
                {
                    CurrentRequestData.ErrorSignal.Raise(ex);

                    Thread.Sleep(120000);
                }
            }
            return submissionIds;
        }
        public void CheckIfRequestsWereProcessed(AmazonSyncModel model, AmazonListingGroup item, List<string> submissionIds)
        {
            var uploadSuccess = false;
            var retryCount = 0;

            var feedContent = _amazonFeedsApiService.GetProductsImageFeeds(item);
            while (!uploadSuccess)
            {
                retryCount++;
                if (retryCount == 5)
                {
                    AmazonProgressBarHelper.Update(model.Task, "Error",
                                                  "Request timed out. Please check logs for potential errors and try again later.", 100,
                                                  100);
                    break;
                }

                try
                {
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Checking if request was processed...", 100, 75);
                    if (_amazonFeedsApiService.GetFeedSubmissionList(submissionIds.First()).FeedProcessingStatus ==
                        "_DONE_")
                    {
                        AmazonProgressBarHelper.Update(model.Task, "Push", "Request was processed", 100, 75);
                        foreach (var amazonListing in item.Items)
                        {
                            AmazonProgressBarHelper.Update(model.Task, "Push", "Updating local status of Amazon Listing with SKU:" + amazonListing.SellerSKU, 100, 85);
                            _amazonListingService.UpdateAmazonListingStatusAndAsin(amazonListing, null);
                        }

                        SubmitProductImageFeed(model, feedContent, ref submissionIds);

                        uploadSuccess = true;
                    }
                    else
                    {
                        AmazonProgressBarHelper.Update(model.Task, "Push",
                                                       "Nothing yet, we will wait 2 min. more and try again...", 100, 75);

                        Thread.Sleep(120000);
                    }
                }
                catch (Exception ex)
                {
                    CurrentRequestData.ErrorSignal.Raise(ex);

                    AmazonProgressBarHelper.Update(model.Task, "Push",
                                                   "Amazon Api is busy, we will wait additional 2 min. and try again...", 100,
                                                   75);

                    Thread.Sleep(120000);
                }
            }
        }
        public void CheckIfRequestWasProcessed(AmazonSyncModel model, AmazonListing amazonListing, List<string> submissionIds)
        {
            var uploadSuccess = false;
            var retryCount = 0;

            var feedContent = _amazonFeedsApiService.GetSingleProductImageFeed(amazonListing);
            while (!uploadSuccess)
            {
                retryCount++;
                if (retryCount == 3)
                {
                    AmazonProgressBarHelper.Update(model.Task, "Error",
                                                  "Request timed out. Please check logs for potential errors and try again later.", 100,
                                                  100);
                    break;
                }

                try
                {
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Checking if request was processed...", 100, 75);
                    var amazonProduct = _amazonProductsApiService.GetMatchingProductForId(amazonListing.SellerSKU);
                    if (amazonProduct != null)
                    {
                        AmazonProgressBarHelper.Update(model.Task, "Push", "Request was processed", 100, 75);
                        AmazonProgressBarHelper.Update(model.Task, "Push", "Updating local status of Amazon Listing with SKU:" + amazonListing.SellerSKU, 100, 85);
                        _amazonListingService.UpdateAmazonListingStatusAndAsin(amazonListing, amazonProduct);

                        if (feedContent != null)
                        {
                            SubmitProductImageFeed(model, feedContent, ref submissionIds);
                        }

                        uploadSuccess = true;
                    }
                    else
                    {
                        AmazonProgressBarHelper.Update(model.Task, "Push",
                                                       "Nothing yet, we will wait 2 min. more and try again...", 100, 75);
                        Thread.Sleep(120000);
                    }
                }
                catch (Exception ex)
                {
                    CurrentRequestData.ErrorSignal.Raise(ex);

                    AmazonProgressBarHelper.Update(model.Task, "Push",
                                                   "Amazon Api is busy, we will wait additional 2 min. and try again...", 100,
                                                   75);
                    Thread.Sleep(120000);
                }
            }
        }

        public string SubmitCloseRequest(AmazonSyncModel model, FileStream productFeedContent)
        {
            var submissionId = String.Empty;
            var uploadSuccess = false;
            var retryCount = 0;
            while (!uploadSuccess)
            {
                retryCount++;
                if (retryCount == 3)
                {
                    AmazonProgressBarHelper.Update(model.Task, "Error",
                                                  "Request timed out. Please check logs for potential errors and try again later.", 100,
                                                  100);
                    break;
                }

                try
                {
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing request to Amazon", 100, 0);
                    var feedResponse = _amazonFeedsApiService.SubmitFeed(AmazonFeedType._POST_PRODUCT_DATA_, productFeedContent);
                    submissionId = feedResponse.FeedSubmissionId;
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Request pushed to Amazon", 100, 75);

                    uploadSuccess = true;
                }
                catch (Exception ex)
                {
                    _amazonLogService.Add(AmazonLogType.Listings, AmazonLogStatus.Error, ex, null,
                                          AmazonApiSection.Feeds, null, null, null, null, "Error during push of product delete request to Amazon");

                    Thread.Sleep(120000);
                }
            }
            return submissionId;
        }
        public void CheckIfDeleteRequestWasProcessed(AmazonSyncModel model, AmazonListing amazonListing, string submissionId)
        {
            var uploadSuccess = false;
            var retryCount = 0;

            while (!uploadSuccess)
            {
                retryCount++;
                if (retryCount == 3)
                {
                    AmazonProgressBarHelper.Update(model.Task, "Error",
                                                  "Request timed out. Please check logs for potential errors and try again later.", 100,
                                                  100);
                    break;
                }

                try
                {
                    AmazonProgressBarHelper.Update(model.Task, "Push", "Checking if request was processed...", 100, 75);
                    if (_amazonFeedsApiService.GetFeedSubmissionList(submissionId).FeedProcessingStatus == "_DONE_")
                    {
                        AmazonProgressBarHelper.Update(model.Task, "Push", "Request was processed", 100, 75);
                        AmazonProgressBarHelper.Update(model.Task, "Push",
                                                       "Updating local status of Amazon Listing with SKU:" + amazonListing.SellerSKU,
                                                       100, 85);
                        _amazonListingService.UpdateAmazonListingStatus(amazonListing);

                        uploadSuccess = true;
                    }
                    else
                    {
                        AmazonProgressBarHelper.Update(model.Task, "Push",
                                                       "Nothing yet, we will wait 2 min. more and try again...", 100, 75);
                        Thread.Sleep(120000);
                    }
                }
                catch (Exception ex)
                {
                    _amazonLogService.Add(AmazonLogType.Listings, AmazonLogStatus.Error, ex, null,
                                          AmazonApiSection.Feeds, null, null, amazonListing, null);

                    AmazonProgressBarHelper.Update(model.Task, "Push",
                                                   "Amazon Api is busy, we will need to wait additional 2 min. and try again",
                                                   100, 75);
                    Thread.Sleep(120000);
                }
            }
        }

        private void SubmitProductFeed(AmazonSyncModel model, FileStream feedContent, ref List<string> submissionIds)
        {
            if (feedContent == null) return;

            AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing product details", 100, 0);
            var feedResponse = _amazonFeedsApiService.SubmitFeed(AmazonFeedType._POST_PRODUCT_DATA_, feedContent);
            var submissionId = feedResponse.FeedSubmissionId;
            submissionIds.Add(submissionId);
            AmazonProgressBarHelper.Update(model.Task, "Push", "Product details pushed", 100, 25);
        }
        private void SubmitProductPriceFeed(AmazonSyncModel model, FileStream feedContent, ref List<string> submissionIds)
        {
            if (feedContent == null) return;

            AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing product pricing information", 100, 25);
            var feedResponse = _amazonFeedsApiService.SubmitFeed(AmazonFeedType._POST_PRODUCT_PRICING_DATA_, feedContent);
            var submissionId = feedResponse.FeedSubmissionId;
            submissionIds.Add(submissionId);
            AmazonProgressBarHelper.Update(model.Task, "Push", "Product pricing information pushed", 100, 50);
        }
        private void SubmitProductInventoryFeed(AmazonSyncModel model, FileStream feedContent, ref List<string> submissionIds)
        {
            if (feedContent == null) return;

            AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing product inventory information", 100, 50);
            var feedResponse = _amazonFeedsApiService.SubmitFeed(AmazonFeedType._POST_INVENTORY_AVAILABILITY_DATA_, feedContent);
            var submissionId = feedResponse.FeedSubmissionId;
            submissionIds.Add(submissionId);
            AmazonProgressBarHelper.Update(model.Task, "Push", "Product inventory information pushed", 100, 75);
        }
        private void SubmitProductImageFeed(AmazonSyncModel model, FileStream feedContent, ref List<string> submissionIds)
        {
            if (feedContent == null) return;

            AmazonProgressBarHelper.Update(model.Task, "Push", "Pushing product image", 100, 85);
            var feedResponse = _amazonFeedsApiService.SubmitFeed(AmazonFeedType._POST_PRODUCT_IMAGE_DATA_, feedContent);
            var submissionId = feedResponse.FeedSubmissionId;
            submissionIds.Add(submissionId);
            AmazonProgressBarHelper.Update(model.Task, "Push", "Product image pushed", 100, 100);
        }
    }

    public interface IAmazonOrderRequestService
    {
        string SubmitOrderFulfillmentFeed(FileStream feedContent);
    }

    public class AmazonOrderRequestService : IAmazonOrderRequestService
    {
        private readonly IAmazonFeedsApiService _amazonFeedsApiService;

        public AmazonOrderRequestService(IAmazonFeedsApiService amazonFeedsApiService)
        {
            _amazonFeedsApiService = amazonFeedsApiService;
        }

        public string SubmitOrderFulfillmentFeed(FileStream feedContent)
        {
            if (feedContent == null) return null;

            var feedResponse = _amazonFeedsApiService.SubmitFeed(AmazonFeedType._POST_ORDER_FULFILLMENT_DATA_, feedContent);
            return feedResponse.FeedSubmissionId;
        }
    }
}