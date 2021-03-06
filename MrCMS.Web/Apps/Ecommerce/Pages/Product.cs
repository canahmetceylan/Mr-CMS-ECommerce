using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.ETags;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Ecommerce.Pages
{
    public class Product : Webpage
    {
        public Product()
        {
            Variants = new List<ProductVariant>();
            SpecificationValues = new List<ProductSpecificationValue>();
            Categories = new List<Category>();
            Options = new List<ProductOption>();
            RelatedProducts = new List<Product>();
        }

        public virtual MediaCategory Gallery { get; set; }

        public virtual bool IsMultiVariant => Variants.Count > 1;

        public virtual IList<ProductVariant> Variants { get; set; }

        public virtual IList<ProductSpecificationValue> SpecificationValues { get; set; }

        public virtual IList<Category> Categories { get; set; }

        public virtual IList<ProductOption> Options { get; set; }

        [Obsolete("Use BrandPage instead")]
        public virtual OldBrand Brand { get; set; }
        public virtual Brand BrandPage { get; set; }

        [DisplayName("Abstract")]
        [StringLength(500, ErrorMessage = "Abstract cannot be longer than 500 characters.")]
        public virtual string ProductAbstract { get; set; }

        [DisplayName("Search Result Abstract"), StringLength(500, ErrorMessage = "Abstract cannot be longer than 500 characters.")]
        public virtual string SearchResultAbstract { get; set; }

        public virtual IEnumerable<MediaFile> Images
        {
            get
            {
                return Gallery != null
                    ? (IEnumerable<MediaFile>)
                        Gallery.Files.Where(file => file.IsImage()).OrderBy(file => file.DisplayOrder)
                    : new List<MediaFile>();
            }
        }

        public virtual string DisplayImageUrl
        {
            get
            {
                return Images.Any()
                    ? Images.First().FileUrl
                    : MrCMSApplication.Get<EcommerceSettings>().DefaultNoProductImage;
            }
        }

        public virtual IList<Product> RelatedProducts { get; set; }


        public virtual IList<Product> PublishedRelatedProducts
        {
            get { return RelatedProducts.Where(x => x.Published).ToList(); }
        }

        public virtual string GetSpecification(string name)
        {
            ProductSpecificationValue spec =
                SpecificationValues.FirstOrDefault(
                    value => value.ProductSpecificationAttributeOption.ProductSpecificationAttribute.Name == name);
            return spec == null ? null : spec.Value;
        }
    }
}