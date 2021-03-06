using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Payment.Braintree
{
    public class BraintreeSettings : SiteSettingsBase
    {
        public BraintreeSettings()
        {
            ThreeDSecureRequired = true;
        }

        [DisplayName("Enabled?")]
        public bool Enabled { get; set; }
        [DisplayName("Use Sandbox?")]
        public bool UseSandbox { get; set; }
        [DisplayName("Merchant Id")]
        public string MerchantId { get; set; }
        [DisplayName("Public Key")]
        public string PublicKey { get; set; }
        [DisplayName("Private Key")]
        public string PrivateKey { get; set; }
        public override bool RenderInSettings
        {
            get { return false; }
        }
        [DisplayName("3D Secure Required?")]
        public bool ThreeDSecureRequired { get; set; }
        [DisplayName("Paypal Payment Enabled?")]
        public bool PayPalPaymentEnabled { get; set; }
    }
}