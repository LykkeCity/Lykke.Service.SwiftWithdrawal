using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

namespace Lykke.Service.SwiftWithdrawal.Contracts
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class SwiftCashoutStateChangedEvent
    {
        public string Changer { get; set; }

        public string ClientId { get; set; }

        public string RequestId { get; set; }

        public CashoutRequestStatus Status { get; set; }

        public CashoutVolumeSize VolumeSize { get; set; }
    }
}
