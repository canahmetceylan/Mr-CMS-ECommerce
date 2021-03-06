using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Helpers;
using System.Linq;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Ecommerce.Helpers;
using MrCMS.Web.Apps.Ecommerce.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Ecommerce.Services.Misc
{
    public class OptionService : IOptionService
    {
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly ISession _session;

        public OptionService(ISession session, IStringResourceProvider stringResourceProvider)
        {
            _stringResourceProvider = stringResourceProvider;
            _session = session;
        }

        public List<SelectListItem> GetEnumOptions<T>() where T : struct
        {
            return
                Enum.GetValues(typeof(T))
                    .Cast<T>()
                    .BuildSelectItemList(item => GeneralHelper.GetDescriptionFromEnum(item as Enum), item => item.ToString(), emptyItem: null);
        }

        public List<SelectListItem> GetEnumOptionsWithEmpty<T>() where T : struct
        {
            return
                Enum.GetValues(typeof(T))
                    .Cast<T>()
                    .BuildSelectItemList(item => GeneralHelper.GetDescriptionFromEnum(item as Enum), item => item.ToString(), emptyItemText:_stringResourceProvider.GetValue("Please select..."));
        }

        public IList<SelectListItem> GetCategoryOptions()
        {
            return _session.QueryOver<Category>().List().BuildSelectItemList(item => item.Name, item => item.Id.ToString(), null, new SelectListItem {Text = _stringResourceProvider.GetValue("Please select...")});
        }
    }
}