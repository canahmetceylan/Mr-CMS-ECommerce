namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public class ProductCardModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Abstract { get; set; }
        public decimal? Price { get; set; }
        public decimal? PreviousPrice { get; set; }
        public string Image { get; set; }
        public int? VariantId { get; set; }
        public string PreviousPriceText { get; set; }
    }
}