using Lykke.Service.SwiftWithdrawal.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.SwiftWithdrawal.Core.Domain.Cashout
{
    public interface ICashoutRequest
    {
        string Id { get; }
        string AssetId { get; }
        string ClientId { get; }
        double Amount { get; }
        double FeeSize { get; }
        DateTime DateTime { get; }
        bool IsHidden { get; }
        string PaymentSystem { get; }
        string PaymentFields { get; }
        string BlockchainHash { get; }
        CashoutRequestStatus Status { get; }
        TransactionState State { get; }
        CashoutRequestTradeSystem TradeSystem { get; }
        string AccountId { get; }
        CashoutVolumeSize VolumeSize { get; set; }
        string PreviousId { get; }
    }

    public class SwiftCashOutRequest : ICashoutRequest
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string AssetId { get; set; }
        public string PaymentSystem { get; set; }
        public string PaymentFields { get; set; }
        public string BlockchainHash { get; set; }
        public CashoutRequestStatus Status { get; set; }
        public TransactionState State { get; set; }
        public CashoutRequestTradeSystem TradeSystem { get; set; }
        public double Amount { get; set; }
        public double FeeSize { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsHidden { get; set; }
        public string AccountId { get; set; }
        public CashoutVolumeSize VolumeSize { get; set; }
        public string PreviousId { get; set; }
    }

    public sealed class PaymentSystem
    {
        public static readonly PaymentSystem Swift = new PaymentSystem("SWIFT");

        private PaymentSystem(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(PaymentSystem paymentSystem)
        {
            return paymentSystem.ToString();
        }
    }
}
