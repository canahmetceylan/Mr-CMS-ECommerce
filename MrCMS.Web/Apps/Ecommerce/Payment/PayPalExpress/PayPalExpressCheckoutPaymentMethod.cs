using MrCMS.Web.Apps.Ecommerce.Models;

namespace MrCMS.Web.Apps.Ecommerce.Payment.PayPalExpress
{
    public class PayPalExpressCheckoutPaymentMethod : BasePaymentMethod
    {
        public const string MethodSystemName = "PayPalExpressCheckout";
        private readonly PayPalExpressCheckoutSettings _payPalExpressCheckoutSettings;

        public PayPalExpressCheckoutPaymentMethod(PayPalExpressCheckoutSettings payPalExpressCheckoutSettings)
        {
            _payPalExpressCheckoutSettings = payPalExpressCheckoutSettings;
        }

        public override string Name
        {
            get { return "PayPal Express Checkout"; }
        }

        public override string SystemName
        {
            get { return MethodSystemName; }
        }

        public override string ControllerName
        {
            get { return MethodSystemName; }
        }

        public override string ActionName
        {
            get { return "Form"; }
        }

        public override PaymentType PaymentType
        {
            get { return PaymentType.PayPalExpress; }
        }

        public override bool Enabled
        {
            get { return _payPalExpressCheckoutSettings.Enabled; }
        }

        protected override bool StandardCanUseLogic(CartModel cart)
        {
            return cart.IsPayPalTransaction;
        }
    }
}