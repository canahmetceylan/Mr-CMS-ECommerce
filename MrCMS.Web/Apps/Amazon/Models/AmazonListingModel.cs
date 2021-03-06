using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Paging;
using MrCMS.Web.Apps.Amazon.Entities.Listings;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public class AmazonListingModel
    {
        public AmazonListingModel()
        {
            Listing = new AmazonListing();

            Name = String.Empty;
            Page = 1;
            PageSize = 10;

            CategoryId = 0;
            Categories = new List<SelectListItem>();
            ProductVariants = new PagedList<ProductVariant>(new List<ProductVariant>(), Page, PageSize);
        }

        //Listing
        public AmazonListingGroup AmazonListingGroup { get; set; }
        public AmazonListing Listing { get; set; }
        public string ChosenProductVariants { get; set; }

        //Search & Paging
        public string Name { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        //Product Variants
        public int CategoryId { get; set; }
        public IList<SelectListItem> Categories { get; set; }
        public IPagedList<ProductVariant> ProductVariants { get; set; }
    }

}