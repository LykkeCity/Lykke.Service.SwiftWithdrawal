using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.SwiftWithdrawal.Core.Domain.Cashout
{
    public interface ICashoutPaymentDate
    {
        DateTime PaymentDate { get; }
        string RequestId { get; }
    }

    public interface ICashoutPaymentDateRepository
    {
        Task<ICashoutPaymentDate> GetAsync(string requestId);
        Task AddAsync(string requestId, DateTime paymentDate);
        Task<bool> DeleteIfExistAsync(string requestId);
    }
}
