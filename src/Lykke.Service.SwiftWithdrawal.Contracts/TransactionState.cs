using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.SwiftWithdrawal.Contracts
{
    /// <summary>
    /// Transaction state
    /// </summary>
    public enum TransactionState
    {
        InProcessOnchain,
        SettledOnchain,
        InProcessOffchain,
        SettledOffchain,
        SettledNoChain
    }
}
