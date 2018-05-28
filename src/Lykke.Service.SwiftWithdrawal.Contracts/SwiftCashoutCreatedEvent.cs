﻿using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace Lykke.Service.SwiftWithdrawal.Contracts
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class SwiftCashoutCreatedEvent
    {
        public string ClientId { get; set; }

        public string AssetId { get; set; }

        public decimal Volume { get; set; }

        public CashoutRequestTradeSystem TradeSystem { get; set; }
    }
}
