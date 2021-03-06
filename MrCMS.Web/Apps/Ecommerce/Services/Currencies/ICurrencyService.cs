using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Web.Apps.Ecommerce.Entities.Currencies;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Ecommerce.Services.Currencies
{
    public interface ICurrencyService
    {
        IList<Currency> GetAll();
        void Add(Currency currency);
        void Update(Currency currency);
        void Delete(Currency currency);
        List<SelectListItem> Options(int id);
    }

    public class CurrencyService : ICurrencyService
    {
        private readonly ISession _session;

        public CurrencyService(ISession session)
        {
            _session = session;
        }

        public IList<Currency> GetAll()
        {
            return _session.QueryOver<Currency>().Cacheable().List();
        }

        public void Add(Currency currency)
        {
            _session.Transact(session => session.Save(currency));
        }

        public void Update(Currency currency)
        {
            _session.Transact(session => session.Update(currency));
        }

        public void Delete(Currency currency)
        {
            _session.Transact(session => session.Delete(currency));
        }

        public List<SelectListItem> Options(int id)
        {
            return GetAll().BuildSelectItemList(currency => currency.Name,
                                                currency => currency.Id.ToString(), currency => currency.Id == id,
                                                emptyItem: null);
        }
    }
}