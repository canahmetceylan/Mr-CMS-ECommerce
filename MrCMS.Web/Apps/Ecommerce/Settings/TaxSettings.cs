using System.ComponentModel;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Ecommerce.Settings
{
    public class TaxSettings : SiteSettingsBase
    {
        public TaxSettings()
        {
            TaxCalculationMethod = TaxCalculationMethod.Individual;
        }

        [DisplayName("Input Prices Include Tax")]
        public PriceLoadingMethod PriceLoadingMethod { get; set; }

        public TaxCalculationMethod TaxCalculationMethod { get; set; }

        public ApplyCustomerTax ApplyCustomerTax { get; set; }

        public DiscountOnPrices DiscountOnPrices { get; set; }

        //public bool DiscountsApplyBeforeTax { get; set; }

        [DisplayName("Shipping Rates Include Tax")]
        public bool ShippingRateIncludesTax { get; set; }

        [DisplayName("Shipping Rate Taxes Enabled?")]
        public bool ShippingRateTaxesEnabled { get; set; }

        [DisplayName("Taxes Enabled?")]
        public bool TaxesEnabled { get; set; }

        public override bool RenderInSettings => false;
    }

    public enum DiscountOnPrices
    {
        IncludingTax,
        ExcludingTax
    }

    public enum ApplyCustomerTax
    {
        AfterDiscount,
        BeforeDiscount
    }

    public enum TaxCalculationMethod
    {
        Individual,
        Row
    }

    public enum PriceLoadingMethod
    {
        IncludingTax,
        ExcludingTax
    }
}