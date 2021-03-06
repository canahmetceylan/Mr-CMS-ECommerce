using MrCMS.Web.Apps.Ecommerce.Entities.Geographic;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Services.Currencies;
using MrCMS.Web.Apps.Ecommerce.Services.Geographic;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;

namespace MrCMS.Web.Apps.Ecommerce.Installation.Services
{
    public class SetupCurrency : ISetupCurrency
    {
        private readonly ICurrencyService _currencyService;
        private readonly ITaxRateManager _taxRateManager;
        private readonly ICountryService _countryService;

        public SetupCurrency(ICurrencyService currencyService, ITaxRateManager taxRateManager, ICountryService countryService)
        {
            _currencyService = currencyService;
            _taxRateManager = taxRateManager;
            _countryService = countryService;
        }

        public void Setup()
        {
            //add currency
            var britishCurrency = new Entities.Currencies.Currency
            {
                Name = "British Pound",
                Code = "GBP",
                Format = "?0.00"
            };
            _currencyService.Add(britishCurrency);

            var taxRate = new TaxRate
            {
                Name = "VAT",
                Code = "S",
                Percentage = 20m
            };
            _taxRateManager.Add(taxRate);

            var uk = new Country
            {
                Name = "United Kingdom",
                ISOTwoLetterCode = "GB"
            };
            _countryService.AddCountry(uk);
        }
    }
}