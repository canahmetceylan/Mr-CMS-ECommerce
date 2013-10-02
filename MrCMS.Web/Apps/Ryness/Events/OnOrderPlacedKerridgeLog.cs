﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Web.Apps.Ecommerce.Entities.Orders;
using MrCMS.Web.Apps.Ecommerce.Services.Orders.Events;
using MrCMS.Web.Apps.Ryness.Entities;
using MrCMS.Web.Apps.Ryness.Services;

namespace MrCMS.Web.Apps.Ryness.Events
{
    public class OnOrderPlacedKerridgeLog : IOnOrderPlaced 
    {
        private readonly IKerridgeService _kerridgeService;

        public OnOrderPlacedKerridgeLog(IKerridgeService kerridgeService)
        {
            _kerridgeService = kerridgeService;
        }

        public int Order { get { return 100; } }
        public void OnOrderPlaced(Order order)
        {
            var kerridgeLog = new KerridgeLog
                {
                    Order = order,
                    Sent = false
                };

            _kerridgeService.Add(kerridgeLog);
        }
    }
}