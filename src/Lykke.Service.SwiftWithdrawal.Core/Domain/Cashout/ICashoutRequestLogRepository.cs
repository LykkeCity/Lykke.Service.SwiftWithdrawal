using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.SwiftWithdrawal.Contracts;

namespace Lykke.Service.SwiftWithdrawal.Core.Domain.Cashout
{
    public interface ICashoutRequestLogItem
    {
        DateTime CreationTime { get; }
        string RequestId { get; }
        string Changer { get; }
        string ClientName { get; }
        string ClientEmail { get; }
        CashoutRequestStatus Status { get; }
        CashoutVolumeSize VolumeSize { get; }
    }

    public interface ICashoutRequestLogRepository
    {
        Task AddRecordAsync(string changer, string requestId, string clientName, string clientEmail, CashoutRequestStatus status, CashoutVolumeSize volumeSize);
        Task<IEnumerable<ICashoutRequestLogItem>> GetRecords(string requestId);
        Task<IEnumerable<ICashoutRequestLogItem>> GetRecords(IEnumerable<string> requestIds);
    }
}
