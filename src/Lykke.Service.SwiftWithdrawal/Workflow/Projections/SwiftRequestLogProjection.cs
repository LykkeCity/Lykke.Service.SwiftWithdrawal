using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Common.Chaos;
using Lykke.Service.PersonalData.Contract;
using Lykke.Service.SwiftWithdrawal.Contracts;
using Lykke.Service.SwiftWithdrawal.Core.Domain.Cashout;

namespace Lykke.Service.SwiftWithdrawal.Workflow.Projections
{
    public class SwiftRequestLogProjection
    {
        private readonly ICashoutRequestLogRepository _cashoutRequestLogRepository;
        private readonly IPersonalDataService _personalDataService;

        public SwiftRequestLogProjection(ICashoutRequestLogRepository cashoutRequestLogRepository, IPersonalDataService personalDataService)
        {
            _cashoutRequestLogRepository = cashoutRequestLogRepository;
            _personalDataService = personalDataService;
        }

        public async Task Handle(SwiftCashoutCreatedEvent evt)
        {
            var client = await _personalDataService.GetAsync(evt.ClientId);

            await _cashoutRequestLogRepository.AddRecordAsync("Client", evt.RequestId, client.FullName, client.Email, CashoutRequestStatus.Pending, evt.VolumeSize);
        }
    }
}
