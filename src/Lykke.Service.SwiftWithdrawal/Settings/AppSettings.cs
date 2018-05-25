using JetBrains.Annotations;
using Lykke.Service.SwiftWithdrawal.Settings.ServiceSettings;
using Lykke.Service.SwiftWithdrawal.Settings.SlackNotifications;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.SwiftWithdrawal.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings
    {
        public SwiftWithdrawalSettings SwiftWithdrawalService { get; set; }

        public SlackNotificationsSettings SlackNotifications { get; set; }

        [Optional]
        public MonitoringServiceClientSettings MonitoringServiceClient { get; set; }
    }
}
