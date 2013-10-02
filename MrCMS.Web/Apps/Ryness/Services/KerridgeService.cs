﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ryness.Entities;
using MrCMS.Web.Apps.Ryness.Models;
using MrCMS.Web.Apps.Ryness.Settings;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Ryness.Services
{
    public class KerridgeService : IKerridgeService
    {
        private readonly ISession _session;
        private readonly KerridgeSettings _kerridgeSettings;

        public KerridgeService(ISession session, KerridgeSettings kerridgeSettings)
        {
            _session = session;
            _kerridgeSettings = kerridgeSettings;
        }

        public KerridgeLogPagedList Search(string query = null, int page = 1, int pageSize = 10)
        {
            IPagedList<KerridgeLog> pagedList = _session.Paged(QueryOver.Of<KerridgeLog>().OrderBy(x=>x.Id).Desc, page, pageSize);

            return new KerridgeLogPagedList(pagedList);
        }

        public IList<KerridgeLog> GetAll()
        {
            return _session.QueryOver<KerridgeLog>().Cacheable().List();
        }

        public void Add(KerridgeLog kerridgeLog)
        {
            _session.Transact(session => session.Save(kerridgeLog));
        }

        public void Update(KerridgeLog kerridgeLog)
        {
            _session.Transact(session => session.Update(kerridgeLog));
        }

        public void Delete(KerridgeLog kerridgeLog)
        {
            _session.Transact(session => session.Delete(kerridgeLog));
        }

        public IList<KerridgeLog> GetAllUnsent()
        {
            //Only do 100 orders at a time for speed
            return _session.QueryOver<KerridgeLog>().Where(x => x.Sent == false).Take(100).List();
        }

        public bool CanSendToKerridge(KerridgeLog log)
        {
            if (log == null)
                return false;
            return log.Sent == false;
        }

        public bool SendToKerridge(KerridgeLog kerridgeLog)
        {
            if (!_kerridgeSettings.Enabled)
                return false;

            var o = kerridgeLog.Order;
            if (CanSendToKerridge(kerridgeLog))
            {
                try
                {
                    string strBranch = _kerridgeSettings.KerridgeBranch;
                    string strAccount = _kerridgeSettings.KerridgeAccountNumber;
                    
                    string strReference = "WEB" + o.Id;

                    string strDaterRquired = DateTime.Today.ToShortDateString();
                    string newDate = strDaterRquired.Substring(6, 4) + "-" + strDaterRquired.Substring(3, 2) + "-" + strDaterRquired.Substring(0, 2);

                    string strName = o.ShippingAddress.Name;
                    string strAddressLine1 = o.ShippingAddress.Company + " " + o.ShippingAddress.Address1;
                    string strAddressLine2 = o.ShippingAddress.Address1;
                    string strAddressLine3 = o.ShippingAddress.City;
                    string strAddressLine4 = o.ShippingAddress.StateProvince; ;
                    string strAddressLine5 = o.ShippingAddress.Country != null ? o.ShippingAddress.Country.Name : "";
                    string strPostCode = o.ShippingAddress.PostalCode;
                    string strInstructions = "" + " Tel:" + o.ShippingAddress.PhoneNumber;
                    string strCarriageCode = "";

                    string strLines = "<lines>";
                    strDaterRquired = newDate;

                    var strHeader = "<header>" + "<branch>" + strBranch + "</branch>" + "<account>" + strAccount + "</account>" + "<reference>" + strReference + "</reference>" +
                        "<daterequired>" + strDaterRquired + "</daterequired>" + "<name>" + strName + " " + o.ShippingAddress.PhoneNumber + "</name>" + "<address>" + "<line>" + strAddressLine1 + "</line>" + "<line>" + strAddressLine2 + "</line>" + "<line>" + strAddressLine3 + "</line>" + "<line>" + strAddressLine4 + "</line>" + "<line>" + strAddressLine5 + "</line>" + "<postcode>" + strPostCode + "</postcode>" + "</address>" + "<instructions>" + strInstructions + "</instructions>" + "</header>";

                    IList<OrderLine> orderItems = o.OrderLines;

                    foreach (OrderLine item in orderItems)
                    {
                        strLines = strLines + "<line>";
                        string sku = CheckProductCodeLengeth(item.SKU);
                        if (sku == "")
                            sku = "000240";

                        strLines = strLines + "<product>" + sku + "</product>";
                        strLines = strLines + "<quantity>" + item.Quantity + "</quantity>";
                        strLines = strLines + "<daterequired>" + strDaterRquired + "</daterequired>";
                        strLines = strLines + "</line>";
                    }

                    if (o.ShippingTotal != null)
                        switch (Math.Round((decimal)o.ShippingTotal, 2).ToString())
                        {
                            case "1.20":
                                strCarriageCode = "C00010";
                                break;
                            case "1.40":
                                strCarriageCode = "C00011";
                                break;
                            case "1.60":
                                strCarriageCode = "C00012";
                                break;
                            case "1.80":
                                strCarriageCode = "C00013";
                                break;
                            case "2.00":
                                strCarriageCode = "C00014";
                                break;
                            case "2.20":
                                strCarriageCode = "C00015";
                                break;
                            case "2.40":
                                strCarriageCode = "C00016";
                                break;
                            case "2.60":
                                strCarriageCode = "C00017";
                                break;
                            case "2.80":
                                strCarriageCode = "C00018";
                                break;
                            case "3.00":
                                strCarriageCode = "C00019";
                                break;
                            case "1.2":
                                strCarriageCode = "C00010";
                                break;
                            case "1.4":
                                strCarriageCode = "C00011";
                                break;
                            case "1.6":
                                strCarriageCode = "C00012";
                                break;
                            case "1.8":
                                strCarriageCode = "C00013";
                                break;
                            case "2":
                                strCarriageCode = "C00014";
                                break;
                            case "2.2":
                                strCarriageCode = "C00015";
                                break;
                            case "2.4":
                                strCarriageCode = "C00016";
                                break;
                            case "2.6":
                                strCarriageCode = "C00017";
                                break;
                            case "2.8":
                                strCarriageCode = "C00018";
                                break;
                            case "3":
                                strCarriageCode = "C00019";
                                break;
                            case "0.95":
                                strCarriageCode = "C00002";
                                break;
                            case "1.95":
                                strCarriageCode = "C00002";
                                break;
                            case "3.90":
                                strCarriageCode = "C00008";
                                break;
                            case "3.9":
                                strCarriageCode = "C00008";
                                break;
                            case "4.90":
                                strCarriageCode = "C00004";
                                break;
                            case "4.9":
                                strCarriageCode = "C00004";
                                break;
                            case "5.76":
                                strCarriageCode = "C00005";
                                break;
                            case "0.00":
                                strCarriageCode = "C00006";
                                break;
                            case "0":
                                strCarriageCode = "C00006";
                                break;
                            default:
                                strCarriageCode = "C00006";
                                break;
                        }

                    strLines = strLines + "<line>";
                    strLines = strLines + "<product>" + strCarriageCode + "</product>";
                    strLines = strLines + "<quantity>1</quantity>";
                    strLines = strLines + "<daterequired>" + strDaterRquired + "</daterequired>";
                    strLines = strLines + "</line>";

                    strLines += "</lines>";

                    string xmlOrderObject = "<kmsgsimple id='" + _kerridgeSettings.Id + "' password='" + _kerridgeSettings.Password + "'>" + strHeader + strLines + "</kmsgsimple>";
                    
                    return CallKerridge(xmlOrderObject);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }

        private string CheckProductCodeLengeth(string sku)
        {
            if (!string.IsNullOrEmpty(sku))
            {
                //------ adds 0 to product number until product number is 6 char long
                if (sku.Length < 6)
                {
                    while ((sku.Length) < 6)
                    {
                        sku = "0" + sku;
                    }
                }

                if (sku.LastIndexOf("-", System.StringComparison.Ordinal) > 0)
                {
                    sku = sku.Substring(0, sku.LastIndexOf("-", System.StringComparison.Ordinal));
                }

                if (sku.Length >= 8)
                {
                    sku = "000240";
                }
            }

            return sku;
        }

        bool CallKerridge(string xmlString)
        {
            string url = _kerridgeSettings.WebServiceUrl;

            url = url + "?xmlstring=" + System.Web.HttpUtility.UrlEncode(xmlString);
            var xmlmessage = "xmlstring=" + System.Web.HttpUtility.HtmlEncode(xmlString);

            WebRequest objRequest = WebRequest.Create(url);

            objRequest.Headers.Add("Action", "PlaceOrder");
            objRequest.Method = "POST";
            objRequest.ContentLength = xmlmessage.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";

            var myWriter = new StreamWriter(objRequest.GetRequestStream());
            myWriter.Write(xmlmessage);

            myWriter.Close();

            try
            {
                objRequest.GetResponse();
            }
            catch (WebException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }

            objRequest.Abort();

            return true;
        }
    }
}