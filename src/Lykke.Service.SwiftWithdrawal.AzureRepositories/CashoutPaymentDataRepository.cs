using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Service.SwiftWithdrawal.Core.Domain.Cashout;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.SwiftWithdrawal.AzureRepositories
{
    public class CashoutPaymentDateEntity : TableEntity, ICashoutPaymentDate
    {
        public DateTime PaymentDate { get; set; }
        public string RequestId { get; set; }
    }

    public class CashoutPaymentDateRepository : ICashoutPaymentDateRepository
    {
        readonly INoSQLTableStorage<CashoutPaymentDateEntity> _tableStorage;

        private static string GetPartitionKey() => "CashoutPaymentDate";

        private static string GetRowKey(string requestId) => requestId;

        public CashoutPaymentDateRepository(INoSQLTableStorage<CashoutPaymentDateEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task AddAsync(string requestId, DateTime paymentDate)
        {
            var entity = new CashoutPaymentDateEntity()
            {
                PartitionKey = GetPartitionKey(),
                RowKey = GetRowKey(requestId),
                RequestId = requestId,
                PaymentDate = paymentDate
            };

            await _tableStorage.InsertOrReplaceAsync(entity);

        }

        public Task<bool> DeleteIfExistAsync(string requestId)
        {
            return _tableStorage.DeleteIfExistAsync(GetPartitionKey(), requestId);
        }

        public async Task<ICashoutPaymentDate> GetAsync(string requestId)
        {
            return await _tableStorage.GetDataAsync(GetPartitionKey(), GetRowKey(requestId));
        }
    }
}
