using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.SwiftWithdrawal.Contracts;

namespace Lykke.Service.SwiftWithdrawal.Core.Domain.Cashout
{
    public interface ICashoutAttemptRepository
    {
        Task<IEnumerable<ICashoutRequest>> GetRequestsAsync(string clientId);
        Task<ICashoutRequest> GetAsync(string clientId, string id);

        Task<bool> TryInsertAsync<T>(string id, ICashoutRequest request, PaymentSystem paymentSystem, T paymentFields, CashoutRequestTradeSystem tradeSystem);
        Task<IEnumerable<ICashoutRequest>> GetAllAttempts();
        Task SetBlockchainHash(string clientId, string requestId, string hash);
        Task<ICashoutRequest> SetPending(string clientId, string requestId);
        Task<ICashoutRequest> SetConfirmed(string clientId, string requestId);
        Task<ICashoutRequest> SetDocsRequested(string clientId, string requestId);
        Task<ICashoutRequest> SetDeclined(string clientId, string requestId);
        Task<ICashoutRequest> SetCanceledByClient(string clientId, string requestId);
        Task<ICashoutRequest> SetCanceledByTimeout(string clientId, string requestId);
        Task<ICashoutRequest> SetProcessed(string clientId, string requestId);
        Task<ICashoutRequest> SetFailed(string clientId, string requestId);
        Task<ICashoutRequest> RollbackHistoryRecord(string clientId, string requestId, CashoutRequestStatus status);
        Task<ICashoutRequest> SetHighVolume(string clientId, string requestId);

        Task SetIsSettledOffchain(string clientId, string requestId);

        Task<IEnumerable<ICashoutRequest>> GetHistoryRecordsAsync(DateTime @from, DateTime to, bool isDescending = true);
        Task<IEnumerable<ICashoutRequest>> GetRelatedRequestsAsync(string requestId);
        Task<IEnumerable<ICashoutRequest>> GetProcessedAttempts();
    }
}
