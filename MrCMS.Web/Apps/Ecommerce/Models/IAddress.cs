
namespace MrCMS.Web.Apps.Ecommerce.Models
{
    public interface IAddress
    {
        string Name { get; }
        string FirstName { get; }
        string LastName { get; }
        string Title { get; }
        string Company { get; }
        string Address1 { get; }
        string Address2 { get; }
        string City { get; }
        string StateProvince { get; }
        string CountryCode { get; }
        string PostalCode { get; }
        string PhoneNumber { get; }
    }
}