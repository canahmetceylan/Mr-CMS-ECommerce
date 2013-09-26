﻿using System;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Amazon.Entities.Logs;
using MrCMS.Web.Apps.Amazon.Entities.Orders;
using MrCMS.Web.Apps.Amazon.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Amazon.Services.Logs
{
    public class AmazonLogService : IAmazonLogService
    {
        private readonly ISession _session;

        public AmazonLogService(ISession session)
        {
            _session = session;
        }

        private AmazonLog Save(AmazonLog log)
        {
            _session.Transact(session => session.SaveOrUpdate(log));
            return log;
        }

        public AmazonLog Add(AmazonLogType type, AmazonLogStatus status,Exception elmahError, MarketplaceWebService.Model.Error amazonError,
            AmazonApiSection? apiSection, AmazonOrder amazonOrder, AmazonListing amazonListing,string apiOperation = "", 
            string message = "", string details = "")
        {
            var log = new AmazonLog()
                {
                    LogType = type,
                    LogStatus = status,
                    ApiSection = apiSection,
                    ApiOperation = !String.IsNullOrWhiteSpace(apiOperation)?apiOperation:null,
                    AmazonOrder = amazonOrder,
                    AmazonListing = amazonListing,
                    Guid = Guid.NewGuid(),
                    Message = !String.IsNullOrWhiteSpace(message) ? message : null,
                    Detail = !String.IsNullOrWhiteSpace(details) ? details : null,
                    Site = CurrentRequestData.CurrentSite
                };
            if (elmahError != null)
            {
                log.Message = elmahError.Message;
                log.Detail = elmahError.StackTrace;
            }
            if (amazonError != null)
            {
                log.ErrorCode = amazonError.Code;
                log.ErrorType = amazonError.Type;
                log.Message = amazonError.Message;
                log.Detail = amazonError.Detail.ToString();
            }

            return Save(log);
        }

        public AmazonLog Add(AmazonLogType type, AmazonLogStatus status, Exception elmahError, MarketplaceWebService.Model.Error amazonError,
            AmazonApiSection? apiSection, string apiOperation = "",
            string message = "", string details = "")
        {
            var log = new AmazonLog()
            {
                LogType = type,
                LogStatus = status,
                ApiSection = apiSection,
                ApiOperation = !String.IsNullOrWhiteSpace(apiOperation) ? apiOperation : null,
                Guid = Guid.NewGuid(),
                Message = !String.IsNullOrWhiteSpace(message) ? message : null,
                Detail = !String.IsNullOrWhiteSpace(details) ? details : null,
                Site = CurrentRequestData.CurrentSite
            };
            if (elmahError != null)
            {
                log.Message = elmahError.Message;
                log.Detail = elmahError.StackTrace;
            }
            if (amazonError != null)
            {
                log.ErrorCode = amazonError.Code;
                log.ErrorType = amazonError.Type;
                log.Message = amazonError.Message;
                log.Detail = amazonError.Detail.ToString();
            }

            return Save(log);
        }

        public AmazonLog Add(AmazonLogType type, AmazonLogStatus status, AmazonApiSection? apiSection, 
            AmazonOrder amazonOrder, AmazonListing amazonListing, string apiOperation = "",
            string message = "", string details = "")
        {
            var log = new AmazonLog()
            {
                LogType = type,
                LogStatus = status,
                ApiSection = apiSection,
                ApiOperation = !String.IsNullOrWhiteSpace(apiOperation) ? apiOperation : null,
                AmazonOrder = amazonOrder,
                AmazonListing = amazonListing,
                Guid = Guid.NewGuid(),
                Message = !String.IsNullOrWhiteSpace(message) ? message : null,
                Detail = !String.IsNullOrWhiteSpace(details) ? details : null,
                Site = CurrentRequestData.CurrentSite
            };

            return Save(log);
        }

        public AmazonLog Add(AmazonLogType type, AmazonLogStatus status, AmazonApiSection? apiSection, string apiOperation = "",
            string message = "", string details = "")
        {
            var log = new AmazonLog()
            {
                LogType = type,
                LogStatus = status,
                ApiSection = apiSection,
                ApiOperation = !String.IsNullOrWhiteSpace(apiOperation) ? apiOperation : null,
                Guid = Guid.NewGuid(),
                Message = !String.IsNullOrWhiteSpace(message) ? message : null,
                Detail = !String.IsNullOrWhiteSpace(details) ? details : null,
                Site = CurrentRequestData.CurrentSite
            };

            return Save(log);
        }

        public AmazonLog Add(AmazonLogType type, AmazonLogStatus status, 
           string message = "", string details = "")
        {
            var log = new AmazonLog()
            {
                LogType = type,
                LogStatus = status,
                Guid = Guid.NewGuid(),
                Message = !String.IsNullOrWhiteSpace(message) ? message : null,
                Detail = !String.IsNullOrWhiteSpace(details) ? details : null,
                Site = CurrentRequestData.CurrentSite
            };

            return Save(log);
        }

        public IPagedList<AmazonLog> GetEntriesPaged(int pageNum, AmazonLogType? type = null, AmazonLogStatus? status = null, int pageSize = 10)
        {
            if(type.HasValue)
                return _session.QueryOver<AmazonLog>()
                           .Where(entry => entry.Site == CurrentRequestData.CurrentSite
                               && (type == null || entry.LogType == type.Value))
                           .OrderBy(entry => entry.Id)
                           .Desc.Paged(pageNum, pageSize);
            if (status.HasValue)
                return _session.QueryOver<AmazonLog>()
                           .Where(entry => entry.Site == CurrentRequestData.CurrentSite
                               && (status == null || entry.LogStatus == status.Value))
                           .OrderBy(entry => entry.Id)
                           .Desc.Paged(pageNum, pageSize);
            return _session.QueryOver<AmazonLog>()
                           .Where(entry => entry.Site == CurrentRequestData.CurrentSite)
                           .OrderBy(entry => entry.Id)
                           .Desc.Paged(pageNum, pageSize);
        }

        public void DeleteAllLogs()
        {
            _session.CreateQuery("DELETE FROM AmazonLog").ExecuteUpdate();
        }
    }
}