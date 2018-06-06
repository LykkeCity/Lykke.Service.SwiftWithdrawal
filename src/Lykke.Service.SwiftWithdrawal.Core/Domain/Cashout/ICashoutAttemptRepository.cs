using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.SwiftWithdrawal.Contracts;

namespace Lykke.Service.SwiftWithdrawal.Core.Domain.Cashout
{
    public interface ICashoutAttemptRepository
    {
        Task<bool> TryInsertAsync<T>(string id, ICashoutRequest request, PaymentSystem paymentSystem, T paymentFields, CashoutRequestTradeSystem tradeSystem);
    }
}
