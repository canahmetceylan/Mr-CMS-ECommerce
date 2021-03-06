using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Entities.Products;
using MrCMS.Web.Apps.Ecommerce.Entities.Tax;
using MrCMS.Web.Apps.Ecommerce.Services.Products;
using MrCMS.Web.Apps.Ecommerce.Services.Tax;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Services
{
    public class TaxRateManagerTests : InMemoryDatabaseTest
    {
        private readonly IGetDefaultTaxRate _getDefaultTaxRate;
        private readonly IProductVariantService _productVariantService;
        private readonly TaxRateManager _taxRateManager;

        public TaxRateManagerTests()
        {
            _productVariantService = A.Fake<IProductVariantService>();
            _getDefaultTaxRate = A.Fake<IGetDefaultTaxRate>();
            _taxRateManager = new TaxRateManager(Session, _productVariantService, _getDefaultTaxRate);
        }

        [Fact]
        public void TaxRateManager_GetAll_ReturnsAllSavedTaxRates()
        {
            List<TaxRate> taxRates = Enumerable.Range(1, 10).Select(i => new TaxRate {Percentage = i}).ToList();
            Session.Transact(session => taxRates.ForEach(rate => session.Save(rate)));

            IList<TaxRate> allRates = _taxRateManager.GetAll();

            allRates.Should().BeEquivalentTo(taxRates);
        }


        [Fact]
        public void TaxRateManager_Add_SavesThePassedTaxRateToSession()
        {
            _taxRateManager.Add(new TaxRate());

            Session.QueryOver<TaxRate>().RowCount().Should().Be(1);
        }

        [Fact]
        public void TaxRateManager_Update_UpdatesTheExistingTaxRate()
        {
            var taxRate = new TaxRate();
            Session.Transact(session => session.Save(taxRate));
            taxRate.Name = "Updated";

            _taxRateManager.Update(taxRate);

            Session.Evict(taxRate);
            Session.Get<TaxRate>(1).Name.Should().Be("Updated");
        }

        [Fact]
        public void TaxRateManager_Delete_RemovesTheTaxRateFromTheSession()
        {
            var taxRate = new TaxRate();
            Session.Transact(session => session.Save(taxRate));

            _taxRateManager.Delete(taxRate);

            Session.QueryOver<TaxRate>().RowCount().Should().Be(0);
        }


        [Fact]
        public void
            TaxRateManager_GetDefaultRateForOrderLine_ShouldReturnDefaultTaxRateIfProductVariantTaxRateNotSpecified()
        {
            var taxRate = new TaxRate {Percentage = 10, IsDefault = true, Name = "GLOBAL", Code = "GL"};
            A.CallTo(() => _getDefaultTaxRate.Get()).Returns(taxRate);

            var orderLine = new OrderLine {ProductVariant = new ProductVariant(), SKU = "123"};

            A.CallTo(() => _productVariantService.GetProductVariantBySKU(orderLine.SKU)).Returns(null);

            TaxRate result = _taxRateManager.GetRateForOrderLine(orderLine);

            result.Should().NotBeNull();
            result.Should().Be(taxRate);
        }

        [Fact]
        public void TaxRateManager_GetDefaultRateForOrderLine_ShouldReturnTaxRate()
        {
            var taxRate = new TaxRate {Percentage = 10, IsDefault = true, Name = "GLOBAL", Code = "GL"};
            var taxRate2 = new TaxRate {Percentage = 50, IsDefault = false, Name = "UK", Code = "UK"};

            _taxRateManager.Add(taxRate);
            _taxRateManager.Add(taxRate2);

            var pv = new ProductVariant {TaxRate = taxRate2};
            var orderLine = new OrderLine {ProductVariant = pv, SKU = "123"};

            A.CallTo(() => _productVariantService.GetProductVariantBySKU(orderLine.SKU)).Returns(pv);

            TaxRate result = _taxRateManager.GetRateForOrderLine(orderLine);

            result.Should().NotBeNull();
            result.Should().Be(taxRate2);
        }

        [Fact]
        public void TaxRateManager_GetDefaultRateForOrderLine_ShouldCallGetProductVariantBySKU()
        {
            var pv = new ProductVariant();
            var orderLine = new OrderLine {ProductVariant = pv, SKU = "123"};

            _taxRateManager.GetRateForOrderLine(orderLine);

            A.CallTo(() => _productVariantService.GetProductVariantBySKU(orderLine.SKU)).MustHaveHappened();
        }
    }
}