using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace Lykke.Service.SwiftWithdrawal.Contracts
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class SwiftCashoutCreateCommand
    {
        public string Id { get; set; }

        public string ClientId { get; set; }

        public string AssetId { get; set; }

        public decimal Volume { get; set; }

        public string AccountId { get; set; }

        public TransactionState State { get; set; }

        public CashoutRequestTradeSystem TradeSystem { get; set; }

        public SwiftDataModel SwiftData { get; set; }
    }
}
