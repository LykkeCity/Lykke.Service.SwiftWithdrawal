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
                FeeSize = request.FeeSize,
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
    }
}
