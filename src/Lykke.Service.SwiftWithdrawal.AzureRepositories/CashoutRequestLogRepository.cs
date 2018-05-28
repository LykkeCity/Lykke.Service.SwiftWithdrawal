using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.SwiftWithdrawal.Contracts;
using Lykke.Service.SwiftWithdrawal.Core.Domain.Cashout;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.SwiftWithdrawal.AzureRepositories
{
    public class CashoutRequestLogRecord : TableEntity, ICashoutRequestLogItem
    {
        public static string GeneratePartitionKey(string requestId) => $"CashoutRequestStatus_{requestId}";

        public static CashoutRequestLogRecord Create(string changer, string requestId, string clientName, string clientEmail, CashoutRequestStatus status, CashoutVolumeSize volumeSize)
        {
            var logRecord = new CashoutRequestLogRecord
            {
                CreationTime = DateTime.UtcNow,
                PartitionKey = GeneratePartitionKey(requestId),
                RequestId = requestId,
                Changer = changer,
                ClientName = clientName,
                ClientEmail = clientEmail,
                Status = status,
                VolumeSize = volumeSize
            };


            return logRecord;
        }


        public DateTime CreationTime { get; set; }
        public string RequestId { get; set; }

        public string Changer { get; set; }

        public string ClientName { get; set; }

        public string ClientEmail { get; set; }

        public CashoutRequestStatus Status { get; set; }

        public string StatusText
        {
            get => Status.ToString();
            set
            {
                CashoutRequestStatus status;
                Enum.TryParse(value, out status);
                Status = status;
            }
        }

        public CashoutVolumeSize VolumeSize { get; set; }

        public string VolumeSizeText
        {
            get => VolumeSize.ToString();
            set
            {
                CashoutVolumeSize volumeSize;
                Enum.TryParse(value, out volumeSize);
                VolumeSize = volumeSize;
            }
        }
    }

    public class CashoutRequestLogRepository : ICashoutRequestLogRepository
    {
        readonly INoSQLTableStorage<CashoutRequestLogRecord> _tableStorage;

        public CashoutRequestLogRepository(INoSQLTableStorage<CashoutRequestLogRecord> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task AddRecordAsync(string changer, string requestId, string clientName, string clientEmail, CashoutRequestStatus status, CashoutVolumeSize volumeSize)
        {
            var record = CashoutRequestLogRecord.Create(changer, requestId, clientName, clientEmail, status, volumeSize);
            await _tableStorage.InsertAndGenerateRowKeyAsDateTimeAsync(record, record.CreationTime);
        }

        public async Task<IEnumerable<ICashoutRequestLogItem>> GetRecords(string requestId)
        {
            var pk = CashoutRequestLogRecord.GeneratePartitionKey(requestId);
            var records = await _tableStorage.GetDataAsync(pk);
            return records;
        }

        public async Task<IEnumerable<ICashoutRequestLogItem>> GetRecords(IEnumerable<string> requestIds)
        {
            var pks = requestIds.Select(CashoutRequestLogRecord.GeneratePartitionKey);
            var records = await _tableStorage.GetDataAsync(pks);
            return records;
        }
    }
}
