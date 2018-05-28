using System;
using System.Collections.Generic;
using System.Text;
using Common;
using Lykke.Service.SwiftWithdrawal.Contracts;
using Lykke.Service.SwiftWithdrawal.Core.Domain.Cashout;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.SwiftWithdrawal.AzureRepositories
{
    public abstract class CashoutBaseEntity : TableEntity, ICashoutRequest
    {
        public string Id => RowKey;
        public string ClientId { get; set; }
        public string AssetId { get; set; }
        public string PaymentSystem { get; set; }
        public string PaymentFields { get; set; }
        public string BlockchainHash { get; set; }
        public string TradeSystem { get; set; }
        CashoutRequestTradeSystem ICashoutRequest.TradeSystem => TradeSystem.ParseEnum(CashoutRequestTradeSystem.Spot);
        public string AccountId { get; set; }

        public CashoutRequestStatus Status
        {
            get { return (CashoutRequestStatus)StatusVal; }
            set { StatusVal = (int)value; }
        }

        public TransactionState State
        {
            get { return (TransactionState)StateVal; }
            set { StateVal = (int)value; }
        }

        public double Amount { get; set; }
        public double FeeSize { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsHidden { get; set; }

        public int StatusVal { get; set; }
        public int StateVal { get; set; }
        public CashoutVolumeSize VolumeSize { get; set; }
        public string VolumeText
        {
            get { return VolumeSize.ToString(); }
            set
            {
                CashoutVolumeSize volumeSize;
                if (Enum.TryParse(value, out volumeSize))
                    VolumeSize = volumeSize;
                else
                    VolumeSize = CashoutVolumeSize.Unknown;
            }
        }
        public string PreviousId { get; set; }
    }
}
