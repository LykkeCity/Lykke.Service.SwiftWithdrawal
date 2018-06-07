using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.SwiftWithdrawal.Contracts
{
    /// <summary>
    /// Swift cashout request status
    /// </summary>
    public enum CashoutRequestStatus
    {
        ClientConfirmation = 4,
        Pending = 0,
        RequestForDocs = 7,
        Confirmed = 1,
        Declined = 2,
        CanceledByClient = 5,
        CanceledByTimeout = 6,
        Processed = 3,
        Failed = 8
    }
}
