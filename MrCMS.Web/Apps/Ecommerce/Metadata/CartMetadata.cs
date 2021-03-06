using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Ecommerce.Pages;

namespace MrCMS.Web.Apps.Ecommerce.Metadata
{
    public class CartMetadata : DocumentMetadataMap<Cart>
    {
        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }

        public override IEnumerable<Type> ChildrenList
        {
            get
            {
                yield return typeof (EnterOrderEmail);
                yield return typeof (OrderPlaced);
                yield return typeof (SetShippingDetails);
                yield return typeof (PaymentDetails);
                yield return typeof (ProductAddedToCart);
            }
        }

        public override bool RequiresParent { get { return false; } }
        public override bool AutoBlacklist { get { return true; } }

        public override string App
        {
            get { return "Ecommerce"; }
        }

        public override string WebGetController
        {
            get { return "Cart"; }
        }
        public override string WebGetAction
        {
            get { return "Show"; }
        }
        public override bool ChildrenMaintainHierarchy
        {
            get { return false; }
        }
    }
}