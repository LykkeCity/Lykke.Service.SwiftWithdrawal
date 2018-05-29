using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Cqrs;
using Lykke.Service.SwiftWithdrawal.Contracts;
using Lykke.Service.SwiftWithdrawal.Core.Domain;
using Lykke.Service.SwiftWithdrawal.Core.Domain.Cashout;
using Lykke.Service.SwiftWithdrawal.Core.Services;

namespace Lykke.Service.SwiftWithdrawal.Workflow.Handlers
{
    public class SwiftCashoutRequestCommandHandler
    {
        private readonly IWithdrawLimitsRepository _withdrawLimitsRepository;
        private readonly ICashoutAttemptRepository _cashoutAttemptRepository;

        public SwiftCashoutRequestCommandHandler(
            IWithdrawLimitsRepository withdrawLimitsRepository,
            ICashoutAttemptRepository cashoutAttemptRepository)
        {
            _withdrawLimitsRepository = withdrawLimitsRepository;
            _cashoutAttemptRepository = cashoutAttemptRepository;
        }

        public async Task<CommandHandlingResult> Handle(SwiftCashoutCreateCommand command, IEventPublisher eventPublisher)
        {
            var amount = (double)command.Volume;

            var model = new SwiftCashOutRequest
            {
                ClientId = command.ClientId,
                AssetId = command.AssetId,
                Amount = amount,
                AccountId = command.AccountId,
                State = command.State,
                Status = CashoutRequestStatus.Pending,
                VolumeSize = await FillVolumeSize(command.AssetId, amount)
            };

            var inserted = await _cashoutAttemptRepository.TryInsertAsync(command.Id, model, PaymentSystem.Swift, command.SwiftData, command.TradeSystem);

            if (inserted)
            {
                eventPublisher.PublishEvent(new SwiftCashoutStateChangedEvent
                {
                    Changer = "Client",
                    ClientId = command.ClientId,
                    RequestId = command.Id,
                    Status = model.Status,
                    VolumeSize = model.VolumeSize
                });

                eventPublisher.PublishEvent(new SwiftCashoutCreatedEvent
                {
                    Volume = command.Volume,
                    AssetId = command.AssetId,
                    ClientId = command.ClientId,
                    TradeSystem = command.TradeSystem
                });
            }

            return CommandHandlingResult.Ok();
        }



        private async Task<CashoutVolumeSize> FillVolumeSize(string assetId, double amount)
        {
            return (await _withdrawLimitsRepository.GetLimitByAssetAsync(assetId)) <= amount
                ? CashoutVolumeSize.High
                : CashoutVolumeSize.Low;
        }
    }
}
