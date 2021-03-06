using MrCMS.Installation;
using MrCMS.Web.Apps.Ecommerce.Installation.Models;
using MrCMS.Web.Apps.Ecommerce.Installation.Services;

namespace MrCMS.Web.Apps.Ecommerce
{
    public class EcommerceAppInstallation : IOnInstallation
    {
        private readonly IImportDummyCategories _importDummyCategories;
        private readonly IImportDummyProducts _importDummyProducts;
        private readonly IIndexSetup _indexSetup;
        private readonly ISetupBaseDocuments _setupBaseDocuments;
        private readonly ISetupCurrency _setupCurrency;
        private readonly ISetupEcommerceLayouts _setupEcommerceLayouts;
        private readonly ISetupEcommerceMedia _setupEcommerceMedia;
        private readonly ISetupEcommerceSettings _setupEcommerceSettings;
        private readonly ISetupEcommerceWidgets _setupEcommerceWidgets;
        private readonly ISetupNewsletterTemplate _setupNewsletterTemplate;

        public EcommerceAppInstallation(ISetupEcommerceLayouts setupEcommerceLayouts,
            ISetupEcommerceMedia setupEcommerceMedia, ISetupBaseDocuments setupBaseDocuments,
            ISetupEcommerceSettings setupEcommerceSettings, ISetupCurrency setupCurrency,
            IImportDummyCategories importDummyCategories, IImportDummyProducts importDummyProducts,
            ISetupEcommerceWidgets setupEcommerceWidgets, IIndexSetup indexSetup,
            ISetupNewsletterTemplate setupNewsletterTemplate)
        {
            _setupEcommerceLayouts = setupEcommerceLayouts;
            _setupEcommerceMedia = setupEcommerceMedia;
            _setupBaseDocuments = setupBaseDocuments;
            _setupEcommerceSettings = setupEcommerceSettings;
            _setupCurrency = setupCurrency;
            _importDummyCategories = importDummyCategories;
            _importDummyProducts = importDummyProducts;
            _setupEcommerceWidgets = setupEcommerceWidgets;
            _indexSetup = indexSetup;
            _setupNewsletterTemplate = setupNewsletterTemplate;
        }

        public int Priority
        {
            get { return 100; }
        }

        public void Install(InstallModel model)
        {
            MediaModel mediaModel = _setupEcommerceMedia.Setup();
            LayoutModel layoutModel = _setupEcommerceLayouts.Setup(mediaModel);
            PageModel pageModel = _setupBaseDocuments.Setup(mediaModel);
            _setupEcommerceSettings.Setup(mediaModel);
            _setupCurrency.Setup();
            _importDummyCategories.Import(mediaModel);
            _importDummyProducts.Import();
            _setupEcommerceWidgets.Setup(pageModel, mediaModel, layoutModel);
            _setupNewsletterTemplate.Setup();
            _indexSetup.ReIndex();
        }
    }
}