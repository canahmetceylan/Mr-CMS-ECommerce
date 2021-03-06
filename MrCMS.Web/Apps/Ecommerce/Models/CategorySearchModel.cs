using System.Collections.Generic;
using MrCMS.Web.Apps.Ecommerce.Pages;
using System.Linq;

namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class CategorySearchModel
    {
        public CategorySearchModel()
        {
            Hierarchy = new List<Category>();
            Children = new List<Category>();
        }

        public List<Category> Hierarchy { get; set; }
        public List<Category> Children { get; set; }

        public bool Any()
        {
            return Hierarchy.Any() || Children.Any();
        }
    }
}