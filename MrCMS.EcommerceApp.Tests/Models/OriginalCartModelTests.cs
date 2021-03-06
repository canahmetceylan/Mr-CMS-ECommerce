using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.EcommerceApp.Tests.Builders;
using MrCMS.Web.Apps.Ecommerce.Entities.Cart;
using MrCMS.Web.Apps.Ecommerce.Models;
using Xunit;

namespace MrCMS.EcommerceApp.Tests.Models
{
    public class OriginalCartModelTests
    {
        [Fact]
        public void CartModel_SubTotal_ShouldBeTheSumOfPricePreTax()
        {
            var cartItem1 = A.Fake<CartItemData>();
            var cartItem2 = A.Fake<CartItemData>();
            A.CallTo(() => cartItem1.PricePreTax).Returns(10);
            A.CallTo(() => cartItem2.PricePreTax).Returns(20);
            var cartModel = new CartModel
                                {
                                    Items = new List<CartItemData> { cartItem1, cartItem2 }
                                };

            var subtotal = cartModel.Subtotal;

            subtotal.Should().Be(30);
        }

        [Fact]
        public void CartModel_SubTotal_NoItemsShouldBeZero()
        {
            var cartModel = new CartModel
                                {
                                    Items = new List<CartItemData>()
                                };

            var subtotal = cartModel.Subtotal;

            subtotal.Should().Be(0);
        }

        [Fact]
        public void CartModel_TotalPreDiscount_ShouldBeTheSumOfPrice()
        {
            var cartItem1 = A.Fake<CartItemData>();
            var cartItem2 = A.Fake<CartItemData>();
            A.CallTo(() => cartItem1.Price).Returns(10);
            A.CallTo(() => cartItem2.Price).Returns(20);
            var cartModel = new CartModel
            {
                Items = new List<CartItemData> { cartItem1, cartItem2 }
            };

            var total = cartModel.TotalPreDiscount;

            total.Should().Be(30);
        }

        [Fact]
        public void CartModel_TaxRates_ShouldBreakDownTotalsByTaxRatePercentage()
        {
            var cartItem1 = A.Fake<CartItemData>();
            var cartItem2 = A.Fake<CartItemData>();
            A.CallTo(() => cartItem1.TaxRatePercentage).Returns(0);
            A.CallTo(() => cartItem1.Price).Returns(10);
            A.CallTo(() => cartItem2.TaxRatePercentage).Returns(20);
            A.CallTo(() => cartItem2.Price).Returns(20);
            var cartModel = new CartModel
            {
                Items = new List<CartItemData> { cartItem1, cartItem2 }
            };

            var dictionary = cartModel.TaxRates;

            dictionary[0].Should().Be(10);
            dictionary[20].Should().Be(20);
        }

        [Fact]
        public void CartModel_DiscountAmount_ShouldBe0IfDiscountIsNull()
        {
            var cartModel = new CartModel();

            var discountAmount = cartModel.DiscountAmount;

            discountAmount.Should().Be(0);
        }

        [Fact]
        public void CartModel_Tax_ShouldBeTheSumOfTax()
        {
            var cartItem1 = A.Fake<CartItemData>();
            var cartItem2 = A.Fake<CartItemData>();
            A.CallTo(() => cartItem1.Tax).Returns(10);
            A.CallTo(() => cartItem2.Tax).Returns(20);
            var cartModel = new CartModel
            {
                Items = new List<CartItemData> { cartItem1, cartItem2 }
            };

            var tax = cartModel.Tax;

            tax.Should().Be(30);
        }

        [Fact]
        public void CartModel_Tax_EmptyCartShouldBeZero()
        {
            var cartModel = new CartModel
            {
                Items = new List<CartItemData>()
            };

            var total = cartModel.Tax;

            total.Should().Be(0);
        }

        [Fact]
        public void CartModel_CanCheckout_IfAllItemsCanBeBoughtThenReturnsTrue()
        {
            var cartItem = new CartItemBuilder().CanBuy().Build();
            var cartModel = new CartModelBuilder().WithItems(cartItem).Build();

            var canCheckout = cartModel.CanCheckout;

            canCheckout.Should().BeTrue();
        }

        [Fact]
        public void CartModel_CanCheckout_NoItemsShouldBeFalse()
        {
            var cartModel = new CartModel();

            var canCheckout = cartModel.CanCheckout;

            canCheckout.Should().BeFalse();
        }
    }
}