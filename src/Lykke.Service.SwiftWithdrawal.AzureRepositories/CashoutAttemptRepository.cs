using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Service.SwiftWithdrawal.Contracts;
using Lykke.Service.SwiftWithdrawal.Core.Domain.Cashout;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.SwiftWithdrawal.AzureRepositories
{
    public class CashoutAttemptEntity : CashoutBaseEntity
    {
        public static class PendingRecords
        {
            public static string GeneratePartition(string clientId)
            {
                return clientId;
            }

            public static string GenerateRowKey(string requestId)
            {
                return requestId;
            }

            public static CashoutAttemptEntity Create<T>(string id, ICashoutRequest request, PaymentSystem paymentSystem, T paymentFields, CashoutRequestTradeSystem tradeSystem)
            {
                var entity = CreateEntity(request, paymentSystem, paymentFields, tradeSystem);
                entity.PartitionKey = GeneratePartition(request.ClientId);
                entity.RowKey = GenerateRowKey(id);
                entity.Status = request.Status;
                entity.PreviousId = request.PreviousId;

                return entity;
            }
        }

        public static class HistoryRecords
        {
            public static string GeneratePartition()
            {
                return "Processed";
            }

            public static CashoutAttemptEntity Create<T>(ICashoutRequest request, PaymentSystem paymentSystem,
                T paymentFields, CashoutRequestTradeSystem tradeSystem)
            {
                var entity = CreateEntity(request, paymentSystem, paymentFields, tradeSystem);
                entity.PartitionKey = GeneratePartition();

                return entity;
            }
        }

        public static CashoutAttemptEntity CreateEntity<T>(ICashoutRequest request, PaymentSystem paymentSystem, T paymentFields, CashoutRequestTradeSystem tradeSystem)
        {
            var dt = DateTime.UtcNow;
            return new CashoutAttemptEntity
            {
                AssetId = request.AssetId,
                Amount = request.Amount,
                ClientId = request.ClientId,
                PaymentSystem = paymentSystem,
                PaymentFields = paymentFields.ToJson(),
                DateTime = dt,
                State = request.State,
                TradeSystem = tradeSystem.ToString(),
                AccountId = request.AccountId,
                VolumeSize = request.VolumeSize
            };
        }
    }

    public class CashoutAttemptRepository : ICashoutAttemptRepository
    {
        readonly INoSQLTableStorage<CashoutAttemptEntity> _tableStorage;

        public CashoutAttemptRepository(INoSQLTableStorage<CashoutAttemptEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public Task<bool> TryInsertAsync<T>(string id, ICashoutRequest request, PaymentSystem paymentSystem, T paymentFields, CashoutRequestTradeSystem tradeSystem)
        {
            var entity = CashoutAttemptEntity.PendingRecords.Create(id, request, paymentSystem, paymentFields, tradeSystem);
            return _tableStorage.TryInsertAsync(entity);
        }

        public async Task<IEnumerable<ICashoutRequest>> GetAllAttempts()
        {
            return await _tableStorage.GetDataAsync();
        }

        public Task SetBlockchainHash(string clientId, string requestId, string hash)
        {
            return _tableStorage.MergeAsync(CashoutAttemptEntity.PendingRecords.GeneratePartition(clientId),
                requestId, entity =>
                {
                    entity.BlockchainHash = hash;
                    entity.State = TransactionState.SettledOnchain;
                    return entity;
                });
        }

        public Task SetIsSettledOffchain(string clientId, string requestId)
        {
            return _tableStorage.MergeAsync(CashoutAttemptEntity.PendingRecords.GeneratePartition(clientId),
                requestId, entity =>
                {
                    entity.State = TransactionState.SettledOffchain;
                    return entity;
                });
        }

        public async Task<ICashoutRequest> SetPending(string clientId, string requestId)
        {
            return await _tableStorage.MergeAsync(CashoutAttemptEntity.PendingRecords.GeneratePartition(clientId),
                requestId, entity =>
                {
                    entity.Status = CashoutRequestStatus.Pending;
                    return entity;
                });
        }

        public async Task<ICashoutRequest> SetDocsRequested(string clientId, string requestId)
        {
            return await _tableStorage.MergeAsync(CashoutAttemptEntity.PendingRecords.GeneratePartition(clientId),
                requestId, entity =>
                {
                    entity.Status = CashoutRequestStatus.RequestForDocs;
                    return entity;
                });
        }

        public async Task<ICashoutRequest> SetConfirmed(string clientId, string requestId)
        {
            return await _tableStorage.MergeAsync(CashoutAttemptEntity.PendingRecords.GeneratePartition(clientId),
                requestId, entity =>
                {
                    entity.Status = CashoutRequestStatus.Confirmed;
                    return entity;
                });
        }

        public Task<ICashoutRequest> SetDeclined(string clientId, string requestId)
        {
            return ChangeStatus(clientId, requestId, CashoutRequestStatus.Declined);
        }

        public Task<ICashoutRequest> SetCanceledByClient(string clientId, string requestId)
        {
            return ChangeStatus(clientId, requestId, CashoutRequestStatus.CanceledByClient);
        }

        public Task<ICashoutRequest> SetCanceledByTimeout(string clientId, string requestId)
        {
            return ChangeStatus(clientId, requestId, CashoutRequestStatus.CanceledByTimeout);
        }

        public Task<ICashoutRequest> SetProcessed(string clientId, string requestId)
        {
            return ChangeStatus(clientId, requestId, CashoutRequestStatus.Processed);
        }

        public Task<ICashoutRequest> SetFailed(string clientId, string requestId)
        {
            return ChangeStatus(clientId, requestId, CashoutRequestStatus.Failed);
        }

        public async Task<ICashoutRequest> SetHighVolume(string clientId, string requestId)
        {
            return await _tableStorage.MergeAsync(CashoutAttemptEntity.PendingRecords.GeneratePartition(clientId),
                requestId, entity =>
                {
                    entity.VolumeSize = CashoutVolumeSize.High;
                    return entity;
                });
        }

        private async Task<ICashoutRequest> ChangeStatus(string clientId, string requestId, CashoutRequestStatus status)
        {
            var entity = await _tableStorage.DeleteAsync(CashoutAttemptEntity.PendingRecords.GeneratePartition(clientId),
                CashoutAttemptEntity.PendingRecords.GenerateRowKey(requestId));

            entity.PartitionKey = CashoutAttemptEntity.HistoryRecords.GeneratePartition();
            entity.Status = status;
            entity.PreviousId = requestId;

            return await _tableStorage.InsertAndGenerateRowKeyAsDateTimeAsync(entity, entity.DateTime);
        }

        public async Task<ICashoutRequest> RollbackHistoryRecord(string clientId, string requestId, CashoutRequestStatus status)
        {
            var entity = await _tableStorage.DeleteAsync(CashoutAttemptEntity.HistoryRecords.GeneratePartition(),
                requestId);

            if (entity.PreviousId == null)
            {
                throw new InvalidOperationException("Cannot rollback this record because of PreviousId is null");
            }

            entity.PartitionKey = CashoutAttemptEntity.PendingRecords.GeneratePartition(clientId);
            entity.RowKey = CashoutAttemptEntity.PendingRecords.GenerateRowKey(entity.PreviousId);
            entity.Status = status;

            await _tableStorage.InsertAsync(entity);

            return entity;
        }

        public async Task<IEnumerable<ICashoutRequest>> GetHistoryRecordsAsync(DateTime @from, DateTime to, bool isDescending = true)
        {
            to = to.Date.AddDays(1);
            var partitionKey = CashoutAttemptEntity.HistoryRecords.GeneratePartition();
            var records = await _tableStorage.WhereAsync(partitionKey, from, to, ToIntervalOption.ExcludeTo);
            if (isDescending)
                return records.OrderByDescending(r => r.DateTime);

            return records;
        }

        public async Task<IEnumerable<ICashoutRequest>> GetRequestsAsync(string clientId)
        {
            return await _tableStorage.GetDataAsync(CashoutAttemptEntity.PendingRecords.GeneratePartition(clientId));
        }

        public async Task<ICashoutRequest> GetAsync(string clientId, string requestId)
        {
            return await _tableStorage.GetDataAsync(CashoutAttemptEntity.PendingRecords.GeneratePartition(clientId), requestId);
        }


        public async Task<IEnumerable<ICashoutRequest>> GetRelatedRequestsAsync(string requestId)
        {
            var rowKeyCond = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, requestId);
            var previousIdCond = TableQuery.GenerateFilterCondition(nameof(ICashoutRequest.PreviousId), QueryComparisons.Equal, requestId);
            var query = new TableQuery<CashoutAttemptEntity>
            {
                FilterString = TableQuery.CombineFilters(rowKeyCond, TableOperators.Or, previousIdCond)
            };

            var requests = await _tableStorage.WhereAsync(query);
            return requests;
        }

        public async Task<IEnumerable<ICashoutRequest>> GetProcessedAttempts()
        {
            var partitionKeyCond = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, CashoutAttemptEntity.HistoryRecords.GeneratePartition());
            var statusCond = TableQuery.GenerateFilterConditionForInt(nameof(CashoutAttemptEntity.StatusVal), QueryComparisons.Equal, (int)CashoutRequestStatus.Processed);
            var query = new TableQuery<CashoutAttemptEntity>
            {
                FilterString = TableQuery.CombineFilters(partitionKeyCond, TableOperators.And, statusCond)
            };

            var requests = await _tableStorage.WhereAsync(query);

            return requests;
        }

    }
}
