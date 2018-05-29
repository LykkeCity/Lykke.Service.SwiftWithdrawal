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
    public class SwiftStateChangedProjection
    {
        private readonly ICashoutRequestLogRepository _cashoutRequestLogRepository;
        private readonly IPersonalDataService _personalDataService;

        public SwiftStateChangedProjection(ICashoutRequestLogRepository cashoutRequestLogRepository, IPersonalDataService personalDataService)
        {
            _cashoutRequestLogRepository = cashoutRequestLogRepository;
            _personalDataService = personalDataService;
        }

        public async Task Handle(SwiftCashoutStateChangedEvent evt)
        {
            var client = await _personalDataService.GetAsync(evt.ClientId);

            await _cashoutRequestLogRepository.AddRecordAsync(evt.Changer, evt.RequestId, client.FullName, client.Email, evt.Status, evt.VolumeSize);
        }
    }
}
