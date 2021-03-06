using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Areas.Admin.Models;
using MrCMS.Web.Apps.Ecommerce.Entities.ProductReviews;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace MrCMS.Web.Apps.Ecommerce.Areas.Admin.Services
{
    public class ProductReviewAdminService : IProductReviewAdminService
    {
        private readonly ISession _session;

        public ProductReviewAdminService(ISession session)
        {
            _session = session;
        }

        public void Update(ProductReview productReview)
        {
            _session.Transact(session => session.Update(productReview));
        }

        public void Delete(ProductReview productReview)
        {
            _session.Transact(session => session.Delete(productReview));
        }

        public void BulkAction(ReviewUpdateModel model)
        {
            ProductReviewOperation currentOperation = model.CurrentOperation;

            switch (currentOperation)
            {
                case ProductReviewOperation.Approve:
                    _session.Transact(session =>
                    {
                        foreach (ProductReview item in model.Reviews)
                        {
                            item.Approved = true;
                            Update(item);
                        }
                    });
                    break;
                case ProductReviewOperation.Reject:
                    _session.Transact(session =>
                    {
                        foreach (ProductReview item in model.Reviews)
                        {
                            item.Approved = false;
                            Update(item);
                        }
                    });
                    break;
                case ProductReviewOperation.Delete:
                    _session.Transact(session =>
                    {
                        foreach (ProductReview item in model.Reviews)
                        {
                            Delete(item);
                        }
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public List<SelectListItem> GetApprovalOptions()
        {
            return Enum.GetValues(typeof (ApprovalStatus))
                .Cast<ApprovalStatus>()
                .BuildSelectItemList(status => status.ToString(), emptyItem: null);
        }

        public IPagedList<ProductReview> Search(ProductReviewSearchQuery query)
        {
            IQueryOver<ProductReview, ProductReview> queryOver = _session.QueryOver<ProductReview>();

            switch (query.ApprovalStatus)
            {
                case ApprovalStatus.Pending:
                    queryOver = queryOver.Where(review => review.Approved == null);
                    break;
                case ApprovalStatus.Rejected:
                    queryOver = queryOver.Where(review => review.Approved == false);
                    break;
                case ApprovalStatus.Approved:
                    queryOver = queryOver.Where(review => review.Approved == true);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(query.ProductName))
            {
                ProductVariant productVariantAlias = null;
                Product productAlias = null;

                queryOver = queryOver.JoinAlias(review => review.ProductVariant, () => productVariantAlias)
                    .JoinAlias(() => productVariantAlias.Product, () => productAlias)
                    .Where(
                        () =>
                            productVariantAlias.Name.IsInsensitiveLike(query.ProductName, MatchMode.Anywhere) ||
                            productAlias.Name.IsInsensitiveLike(query.ProductName, MatchMode.Anywhere));
            }
            if (!string.IsNullOrWhiteSpace(query.Email))
            {
                User userAlias = null;
                queryOver =
                    queryOver.JoinAlias(review => review.User, () => userAlias, JoinType.LeftOuterJoin)
                        .Where(
                            review =>
                                (review.Email.IsInsensitiveLike(query.Email, MatchMode.Anywhere)) ||
                                (userAlias.Email.IsInsensitiveLike(query.Email, MatchMode.Anywhere)));
            }
            if (!string.IsNullOrWhiteSpace(query.Title))
                queryOver = queryOver.Where(review => review.Title.IsLike(query.Title, MatchMode.Anywhere));
            if (query.DateFrom.HasValue)
                queryOver = queryOver.Where(review => review.CreatedOn >= query.DateFrom);
            if (query.DateTo.HasValue)
                queryOver = queryOver.Where(review => review.CreatedOn < query.DateTo);

            return
                queryOver.OrderBy(review => review.Approved)
                    .Asc.ThenBy(review => review.CreatedOn)
                    .Desc.Paged(query.Page);
        }
    }
}